using System;
using System.Collections.Concurrent;
using System.Net;
using WhoisNET.Network;

namespace WhoisNET
{
    /// <summary>
    /// Contains utility methods.
    /// </summary>
    public static class Utilities
    {
        const string _suffixListURL = "https://publicsuffix.org/list/public_suffix_list.dat";
        static readonly ConcurrentBag<string> suffixes = [];
        private static readonly string[] separator = ["://"];

        /// <summary>
        /// A basic check to ensure the query being passed is IPv4 or IPv6. 
        /// </summary>
        /// <param name="query">String to check if it's an IP address</param>
        /// <returns>true = valid IP4/6 address, false = is not an IP address</returns>
        public static bool IsIpAddress(string query)
        {
            return IPAddress.TryParse(query, out _);
        }

        /// <summary>
        /// Returns a valid tld from a url/domain
        /// </summary>
        /// <param name="url">The domain/url to extract the tld from.</param>
        /// <returns>Verified TLD</returns>
        public static async Task<string> GetTLD(string url)
        {
            Debug.WriteVerbose($"Method called with parameter: {url}");
            if (!Uri.TryCreate(url.StartsWith("http") ? url : "http://" + url, UriKind.Absolute, out var uri))
                throw new ArgumentException("Invalid URL", nameof(url));

            string domain = uri.Host;
            Debug.WriteVerbose($"Set domain to: {domain}");


            try
            {
                if (suffixes.IsEmpty)
                {
                    Debug.WriteVerbose($"suffixes.IsEmpty = true");
                    await using var client = new HttpHandler(_suffixListURL);
                    await foreach (var line in client.GetContentByLineAsync())
                    {
                        if (!line.StartsWith("//") && !line.TrimStart().StartsWith("*.") && !string.IsNullOrEmpty(line))
                            suffixes.Add(line.Trim());
                    }
                }

                var parts = domain.Split('.');
                Debug.WriteVerbose($"Domain split by .; parts has {parts.Length} value(s)");
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    var tld = string.Join('.', parts.Skip(i));
                    if (suffixes.Contains(tld))
                    {
                        Debug.WriteVerbose($"Found tld: {tld}");
                        return tld;
                    }
                }

                Debug.WriteVerbose($"Returning {parts[^1]}");
                return parts[^1];
            }
            catch (Exception ex)
            {
                Debug.WriteVerbose($"Get exception");
                Debug.ThrowException(ex.Message, exception: ex);
                throw;
            }
        }



        /// <summary>
        /// Performs a basic regex check for possible referrals
        /// </summary>
        /// <param name="data">Response data to check</param>
        /// <param name="cleanResponse">Removes whois:// from the response</param>
        /// <returns>Referral url</returns>
        public static string GetReferral(string data, bool cleanResponse = true)
        {
            var referral = RegexExpressions.GetReferralServer().Match(data);

            Debug.WriteVerbose($"referral.Success: {referral.Success}");

            if (referral.Success)
            {
                var host = referral.Groups["host"].Value;
                var port = referral.Groups["port"].Success ? int.Parse(referral.Groups["port"].Value) : 43;

                Debug.WriteVerbose($"referral.host:port: {host}:{port}");
                return $"{host}:{port}";
            }

            Debug.WriteVerbose($"referral is empty.");
            return string.Empty;
        }




    }
}