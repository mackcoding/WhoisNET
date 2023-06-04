using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WhoisNET
{
    public static class Utilities
    {
        /// <summary>
        /// Extracts the TLD from a URL
        /// </summary>
        /// <param name="Url">The url (ex: mackcoding.com)</param>
        /// <returns>TLD from the URL (com, net, org, etc)</returns>
        public static string GetTLD(string Url)
        {
            if (IsIpAddr(Url)) return Url;

            // Add http if it doesn't have it
            if (!Url.StartsWith("http")) Url = "http://" + Url;

            // Create a URI, and return the output
            if (Uri.TryCreate(Url, UriKind.Absolute, out var _uri))
            {
                string Host = _uri.Host;
                var Parts = Host.Split('.');

                // Split, and return the tld
                if (Parts.Length > 1)
                {
                    return Parts[Parts.Length - 1];
                }
            }

            // Something went wrong, return an empty string
            return string.Empty;
        }

        public static bool IsIpAddr(string Input)
        {
            if (IPAddress.TryParse(Input, out IPAddress? ipAddress))
            {
                return ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ||
                    ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;
            }

            return false;
        }

        public static string GetDomain(string Url)
        {
            if (IsIpAddr(Url)) return Url;

            if (!Url.StartsWith("http")) Url = "http://" + Url;

            try
            {
                Uri uri = new Uri(Url);
                string Host = uri.Host;
                string[] Parts = Host.Split('.');

                if (Parts.Length>= 2)
                    return $"{Parts[Parts.Length-2]}.{Parts[Parts.Length-1]}";
                 

            } catch (UriFormatException)
            {
                throw new UriFormatException();
            }

            return string.Empty;
        }
    }
}
