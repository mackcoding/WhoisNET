using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WhoisNET.Network
{
    public class WhoisResult
    {
        Dictionary<string, List<string>> Tokens;

        public WhoisResult(string WhoisData)
        {
            Tokens = new();

            using (var Reader = new StringReader(WhoisData))
            {
                for (string? Line = Reader.ReadLine(); Line != null; Line = Reader.ReadLine())
                {
                    if (Line != null)
                    {
                        Line = Line.Trim();

                        if (!string.IsNullOrEmpty(Line) &&
                            Line.Contains(":") && !Line.Contains("http:") &&
                            !Line.Contains("https:"))
                        {
                            var Split = Line.Split(':');
                            var Name = Split[0].Trim().ToLower();
                            var Value = Split[1].Trim();
                            List<string> Values = new() { Value };

                            if (!Tokens.TryAdd(Name, Values))
                                if (!Tokens[Name].Contains(Value))
                                    Tokens[Name].Add(Value);
                        }

                    }
                }
            }

            Tokens.Add("raw", new() { WhoisData });
        }




        /* public WhoisResult(string Whois)
         {
             Tokens = new();

             MatchCollection Matches = Regex.Matches(Whois, @"^(\S+):\s*(.*?)$", RegexOptions.Multiline);

             foreach (Match Item in Matches)
             {
                 var Name = Item.Groups[1].Value.ToLower().Trim();
                 var Value = Item.Groups[2].Value.Trim();
                 List<string> Values = new() { Value };

                 if (!Tokens.TryAdd(Name, Values)) // Check if the add failed, if so...
                     if (!Tokens[Name].Contains(Value)) // Multiple values, so add it to the list.
                         Tokens[Name].Add(Value);


                 Debug.WriteDebug($"{Name}:{Value}");
             }
         }*/

        public string GetValue(WhoisTokens Token)
        {
            if (Tokens == null)
                return string.Empty;

            switch (Token)
            {
                case WhoisTokens.Whois:
                case WhoisTokens.Refer:
                    if (Tokens.ContainsKey("whois")) return Tokens["whois"][0];
                    if (Tokens.ContainsKey("refer")) return Tokens["refer"][0];
                    break;
                case WhoisTokens.Comment:
                    if (Tokens.ContainsKey("comment")) return string.Join(Environment.NewLine, Tokens["comment"]);
                    if (Tokens.ContainsKey("remarks")) return string.Join(Environment.NewLine, Tokens["remarks"]);
                    break;
                case WhoisTokens.Raw:
                    return string.Concat(Tokens["raw"]);
            }

            return string.Empty;
        }


    }

    public enum WhoisTokens
    {
        Refer,
        Whois,
        IpRange,
        Organisation,
        Status,
        Changed,
        Source,
        Comment,
        Raw,
        Invalid

    }
}
