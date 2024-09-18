using System.Net;
using System.Net.Sockets;
using WhoisNET.Enums;
using WhoisNET.Network;

namespace WhoisNET
{
    public class Whois
    {
        private static readonly Dictionary<QueryOptions, object> _options = [];

        /// <summary>
        /// Builds an internal option dictionary. 
        /// </summary>
        /// <param name="option"></param>
        /// <param name="value"></param>
        public static void AddQueryOption(QueryOptions option, object value)
        {
            _options.TryAdd(option, value);
        }

        /// <summary>
        /// Clears the query options.
        /// </summary>
        public static void ClearQueryOptions()
        {
            _options.Clear();
        }

        /// <summary>
        /// Performs a lookup, returns the parsed result. 
        /// </summary>
        /// <param name="query">Domain or IP to query</param>
        /// <returns>Parsed output of the whois</returns>
        //public static async Task<string> Lookup(string query)
        // {
        // todo: write a parser for whois information returned
        //     return string.Empty;
        // }

        /// <summary>   
        /// Uses options to customize the query call.
        /// </summary>
        /// <param name="options">Dictionary of options.</param>
        /// <returns>Query results.</returns>
        public static async Task<string> QueryAsync(Dictionary<QueryOptions, object> options)
        {
            string query = options.TryGetValue(QueryOptions.query, out var qvalue) && qvalue is string qstr ? qstr : string.Empty;
            string whoisServer = options.TryGetValue(QueryOptions.host, out var wsvalue) && wsvalue is string wsstr ? wsstr : string.Empty;
            int queryPort = options.TryGetValue(QueryOptions.port, out var pvalue) && pvalue is int pint ? pint : 43;

            bool followReferral = !(options.TryGetValue(QueryOptions.no_recursion, out var frvalue) && frvalue is bool frbool && frbool);

            Debug.SetLogLevel = options.TryGetValue(QueryOptions.debug, out var dvalue) && dvalue is bool dbool && dbool ? LogLevel.Debug : LogLevel.Off;


            return await QueryAsync(query, whoisServer, followReferral, queryPort: queryPort);
        }



        /// <summary>
        /// Executes QueryAsync() when options are set.
        /// </summary>
        /// <returns>Query result</returns>
        public static async Task<string> QueryAsync() => await QueryAsync(_options);


        /// <summary>
        /// Queries for an IP or domain lookup, returns the raw result.
        /// </summary>
        /// <param name="query">IP or domain to query</param>
        /// <param name="whoisServer">Overrides the whois server to use</param>
        /// <param name="followReferral">Follows the whois server referral</param>
        /// <param name="queryPort">Port to run the query on</param>
        /// <returns>Raw whois query data</returns>
        // todo: do we really need followRefer?
        public static async Task<string> QueryAsync(string query, string? whoisServer = null, bool followReferral = true, int retries = 0, int queryPort = 43)
        {
            string? response;
            var server = string.IsNullOrEmpty(whoisServer) ? await FindQueryServerAsync(query) : whoisServer;

            Debug.WriteDebug($"Using query '{query}' with whois server '{server}', following referrals: {followReferral}");

            if (retries > 5)
                Debug.ThrowException("Too many referral retry requests.");


            try
            {
                await using TcpHandler tcp = new(server, queryPort);
                var dataToSend = query;

                dataToSend = $"{CustomQueryCommand(server)}{query}";

                await tcp.WriteAsync(dataToSend);
                response = await tcp.ReadAsync();

                if (followReferral)
                {
                    var referral = Utilities.GetReferral(response);
                    var (host, port) = referral.Split(':') is var parts && parts.Length > 1 && int.TryParse(parts[1], out var parsedPort) ? (parts[0], parsedPort) : (referral, 43);

                    if (CheckIfReferralIsBlacklisted(host))
                        host = string.Empty;

                    if (!string.IsNullOrEmpty(host))
                    {
                        retries++;
                        Debug.WriteDebug($"Following referral '{host}:{port}', attempt #{retries}.");
                        return await QueryAsync(
                            query,
                            whoisServer: host,
                            followReferral: followReferral,
                            retries: retries,
                            queryPort: port);
                    }
                }
            }
            catch (SocketException ex)
            {
                Debug.ThrowException($"{ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.ThrowException($"Exception: {ex.Message}");
                throw;
            }

            return response ?? string.Empty;
        }

        /// <summary>
        /// Determines what whois server to query
        /// </summary>
        /// <param name="query">Query to find the whois server for </param>
        /// <returns>Whois server hostname</returns>
        public static async Task<string> FindQueryServerAsync(string query)
        {
            // todo: make improvements overall with this method
            // todo: add caching, perhaps a sqlite db?

            if (Utilities.IsIpAddress(query))
            {
                Debug.WriteDebug($"Query '{query}' is an IP address.");
                return "whois.iana.org";
            }

            var tld = await Utilities.GetTLD(query);

            Debug.WriteDebug($"Found tld '{tld}'.");

            // todo: research to see if iana.org is the best to default to
            if (string.IsNullOrEmpty(tld))
            {
                Debug.WriteDebug($"The TLD returned empty; using whois.iana.org.");
                return "whois.iana.org";
            }

            try
            {
                var server = await Dns.GetHostEntryAsync($"{tld}.whois-servers.net");
                Debug.WriteDebug($"Queried '{tld}.whois-servers.net' and received '{server.HostName}'.");
                return server.HostName ?? "whois.iana.org"; // todo: perhaps move the hostname to a const instead
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.HostNotFound)
            {
                Debug.WriteDebug($"Unable to find host '{tld}.whois-servers.net'; using default lookup.");
                return "whois.iana.org"; // todo: perhaps move the hostname to a const instead 
            }
            catch (Exception ex)
            {
                Debug.ThrowException(ex.Message, exception: ex);
                throw;
            }
        }

        /// <summary>
        /// Checks if a referral is blacklisted.
        /// </summary>
        /// <param name="host">Referral hostname.</param>
        /// <returns>True/False - true if the referral is blocked, otherwise false.</returns>
        public static bool CheckIfReferralIsBlacklisted(string host)
        {
            return host switch
            {
                "rwhois.mediacomcc.com" => true,
                _ => false,
            };
        }

        /// <summary>
        /// Modifies the query command depending on the whois server
        /// </summary>
        /// <param name="host">Whois server to check</param>
        /// <returns>Modified query command</returns>
        private static string CustomQueryCommand(string host)
        {
            return host switch
            {
                "arin.net" => "n + ",
                _ => ""
            };
        }

    }
}
