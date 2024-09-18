using System;
using System.Collections.Concurrent;
using System.Net;
using WhoisNET.Network;

namespace WhoisNET
{
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
            if (!Uri.TryCreate(url.StartsWith("http") ? url : "http://" + url, UriKind.Absolute, out var uri))
                throw new ArgumentException("Invalid URL", nameof(url));

            string domain = uri.Host;

            try
            {
                if (suffixes.IsEmpty)
                {
                    await using var client = new HttpHandler(_suffixListURL);
                    await foreach (var line in client.GetContentByLineAsync())
                    {
                        if (!line.StartsWith("//") && !line.TrimStart().StartsWith("*."))
                        {
                            suffixes.Add(line.Trim());
                        }
                    }
                }

                var parts = domain.Split('.');
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    var tld = string.Join('.', parts.Skip(i));
                    if (suffixes.Contains(tld))
                    {
                        return tld;
                    }
                }

                return parts[^1];
            }
            catch (Exception ex)
            {
                Debug.WriteDebug($"GetTLD error: {ex.Message}");
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

            if (referral.Success)
            {
                var host = referral.Groups["host"].Value;
                var port = referral.Groups["port"].Success ? int.Parse(referral.Groups["port"].Value) : 43;

                return $"{host}:{port}";
            }

            return string.Empty;
        }




    }
}