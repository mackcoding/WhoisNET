using System.Text.RegularExpressions;

// todo: get rid of this class and write a proper parser

namespace WhoisNET
{
    public partial class RegexExpressions
    {
        [GeneratedRegex(@"(?:refer|whois|ReferralServer|referral):\s*(?<url>(?:rwhois://)?(?<host>[^:/\s]+)(?::(?<port>\d+))?)", RegexOptions.IgnoreCase, "en-US")]
        public static partial Regex GetReferralServer();






        // todo: determine if this is necessary
        [GeneratedRegex(@"APNIC|RIPE|LACNIC|AFRINIC|ARIN", RegexOptions.IgnoreCase, "en-US")]
        public static partial Regex GetRir();

    }
}
