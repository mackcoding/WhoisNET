using System.Net;
using System.Net.Sockets;
using WhoisNET.Network;

namespace WhoisNET
{
    public class Whois
    {
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
        /// Queries for an IP or domain lookup, returns the raw result.
        /// </summary>
        /// <param name="query">IP or domain to query</param>
        /// <param name="whoisServer">Overrides the whois server to use</param>
        /// <param name="followReferral">Follows the whois server referral</param>
        /// <param name="queryPort">Port to run the query on</param>
        /// <returns>Raw whois query data</returns>
        // todo: do we really need followRefer?
        public static async Task<string> QueryAsync(string query, string? whoisServer = null,bool followReferral = true, int retries = 0, int queryPort = 43)
        {
            string? response;
            var server = string.IsNullOrEmpty(whoisServer) ? await FindQueryServerAsync(query) : whoisServer;

            Debug.WriteDebug($"Using query '{query}' with whois server '{server}', following referrals: {followReferral}");

            if (retries > 5)
            {
                // todo: optimize this so we don't need to call two methods to throw an exception
                string msg = "Too many referral retry requests.";
                Debug.WriteException(msg);
                throw new InvalidOperationException(msg);
            }

            try
            {
                using var tcp = new TcpHandler(server, queryPort);
                var dataToSend = query;

                // todo: likely will need to expand this, and move to its own method
                if (server.Contains("arin.net"))
                    dataToSend = $"n + {query}";

                await tcp.WriteAsync(dataToSend);
                response = await tcp.ReadAsync();

                if (followReferral)
                {
                    var referral = Utilities.GetReferral(response);
                    var (host, port) = referral.Split(':') is var parts && parts.Length > 1 && int.TryParse(parts[1], out var parsedPort) ? (parts[0], parsedPort) : (referral, 43);

                    // todo: for some reason 'rwhois.mediacomcc.com' refers to a dead whois server,
                    // so if we find this referral we are going to ignore it
                    // in the future, we should determine why this is happening perhaps implement a 
                    // blocked referral list
                    if (host.Equals("rwhois.mediacomcc.com"))
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
                Debug.WriteException($"{ex.Message}");
                throw ;
            }
            catch (Exception ex)
            {
                Debug.WriteException($"Exception: {ex.Message}");
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
                Debug.WriteException(ex.Message);
                throw;
            }
        }



    }
}
