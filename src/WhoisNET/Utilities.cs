using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
        // todo: move this method to Network, since it makes use of HttpClient
        public static async Task<string> GetTLD(string url)
        {
            if (!url.StartsWith("http"))
                url = "http://" + url;

            string domain = new Uri(url).Host;

            try
            {
                if (suffixes.IsEmpty)
                {
                    /* The line above ensures we are not reading the suffix list
                     * more than once per instance. This block will read the suffix
                     * file line by line adding it to a hashset called suffixes.
                     * This is used to find a valid tld from the domain. 
                     */

                    HttpClient client = new();

                    var stream = await client.GetStreamAsync(_suffixListURL);
                    using var reader = new StreamReader(stream);

                    string? line;

                    while ((line = await reader.ReadLineAsync()) is not null)
                    {
                        // Remove any lines from the suffix list that start with // or *., and trim
                        if (line.StartsWith("//") || line.Trim().StartsWith("*.")) continue;

                        suffixes.Add(line.Trim());
                    }
                }

                var parts = domain.Split('.');

                for (int i = 0; i < parts.Length; i++)
                {
                    var tld = string.Join('.', parts.Skip(i));
                    if (suffixes.Contains(tld))
                        return tld;
                }

                return domain[(domain.LastIndexOf('.') + 1)..];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetTLD: {ex.Message}");
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

            // todo: investigate if this is necessary
            /* if (!referral.Success)
             {
                 referral = RegexExpressions.GetRir().Match(data);
                 if (referral.Success)
                 {
                     return referral.Groups[0].Value switch
                     {
                         "APNIC" => "whois.apnic.net",
                         "RIPE" => "whois.ripe.net",
                         "LACNIC" => "whois.lacnic.net",
                         "AFRINIC" => "whois.afrinic.net",
                         "ARIN" => "whois.arin.net",
                         _ => "whois.iana.org",
                     };
                 }
             }*/

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