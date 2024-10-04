using System.Text.RegularExpressions;

// todo: get rid of this class and write a proper parser

namespace WhoisNET
{
    /// <summary>
    /// Manages generated regex expressions
    /// </summary>
    public partial class RegexExpressions
    {
        [GeneratedRegex(@"(?:refer|whois|ReferralServer|referral|Referer):\s*(?<url>(?:rwhois://)?(?<host>[^:/\s]+)(?::(?<port>\d+))?)", RegexOptions.IgnoreCase, "en-US")]
        public static partial Regex GetReferralServer();

    }
}
