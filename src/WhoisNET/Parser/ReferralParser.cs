using System.Text;

namespace WhoisNET.Parser
{
    /// <summary>
    /// Searches the whois information for a referral server.
    /// This is essential a lightweight version of the WhoisParser.
    /// </summary>
    public static class ReferralParser
    {
        private static readonly IReadOnlyList<string> _keywords = [
            "refer",
            "whois",
            "referralserver",
            "referral",
            "referer"
        ];


        /// <summary>
        /// Searches whois data for a referral server.
        /// </summary>
        /// <param name="data">Whois data to search</param>
        /// <returns>Hostname and port of referral</returns>
        public static (string host, int port) GetReferral(string data)
        {
            using StringReader reader = new(data);

            string? line;
            StringBuilder host = new(),
                port = new();

            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith('%'))
                    line = line[1..];

                string? keyword = FindKeyword(line);

                if (!string.IsNullOrEmpty(keyword))
                {
                    bool bFoundColon = false;
                    line = line[keyword.Length..].TrimStart(':').Trim();

                    line = line.Trim();

                    for (int i = 0; i < line.Length; i++)
                    {
                        char c = line[i];

                        if (char.IsWhiteSpace(c))
                            continue;

                        if (c == '/')
                            break;

                        if (c == ':')
                        {
                            bFoundColon = true;
                            continue;
                        }

                        if (bFoundColon)
                            port.Append(c);
                        else
                            host.Append(c);
                    }
                    break;
                }
            }


            if (!int.TryParse(port.ToString(), out int portnum) || portnum <= 0 || portnum > 65535)
                portnum = 43;

            return (host.ToString(), portnum);
        }

        /// <summary>
        /// Returns a keyword match
        /// </summary>
        /// <param name="line">Line of text to match</param>
        /// <returns>Matched keyword</returns>
        private static string FindKeyword(string line)
        {
            foreach (var keyword in _keywords)
            {
                if (line.StartsWith(keyword, StringComparison.OrdinalIgnoreCase) &&
                    (line.Length == keyword.Length || !char.IsLetterOrDigit(line[keyword.Length])))
                    return keyword;

            }

            return string.Empty;
        }


    }
}
