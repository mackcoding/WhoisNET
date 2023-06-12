using WhoisNET;
using System.Net;
using System.Net.Sockets;
using System.IO;
using WhoisNET.Network;
using System.Text.RegularExpressions;
using WhoisNET.Models;

namespace WhoisNETClient
{
    internal class Program
    {
        static Dictionary<string, string> EmailList = new Dictionary<string, string>();

        static async Task Main(string[] args)
        {
     
            Debug.LoggingLevel = LogLevel.All;
            //Console.WriteLine($"=====\n{await Whois.QueryAsync("8.8.8.8")}");
            await Whois.QueryAsync("8.8.8.8");
            // Console.WriteLine("====================");
           // Console.WriteLine($"{Whois.Query("8.8.8.8")}");
            /*WhoisModel whoisModel = new WhoisModel();
            whoisModel.Load(@"% IANA WHOIS server
                        % for more information on IANA, visit http://www.iana.org
                        % This query returned 1 object

                        refer:        whois.arin.net

                        inetnum:      8.0.0.0 - 8.255.255.255
                        organisation: Administered by ARIN
                        status:       LEGACY

                        whois:        whois.arin.net

                        changed:      1992-12
                        source:       IANA");
            */
         //   Console.WriteLine(QueryTools.GetWhois("google.com"));
            Console.ReadLine();
       
            var Lines = File.ReadAllLines("domains.txt");
            string Result = string.Empty;

            foreach (var Line in Lines)
            {
                var Whois = QueryTools.GetWhois(Line);
                var Abuse = GetAbuse(Whois);


                if (string.IsNullOrEmpty(Abuse))
                {
                    Console.WriteLine($"Not found: {Line} - {Abuse}");
                    File.WriteAllText($"{Line}.txt", Whois);
                    Result += $"{Environment.NewLine}{Line}: {Abuse}";
                } else
                {
                    Console.WriteLine($"{Line}: {Abuse}");
                    Result += $"{Environment.NewLine}{Line}: {Abuse}";
                }
            }

            // Console.WriteLine(QueryTools.GetWhois("8.8.8.8"));
            //Console.WriteLine(QueryTools.GetWhois("https://google.com"));
            //Console.WriteLine(QueryTools.GetWhois("generates.pw"));
            //Console.WriteLine(QueryTools.GetWhois("sre.google"));
            File.WriteAllText("final.txt", Result);
            Console.ReadLine();
        }

        static string GetRange(string Whois)
        {
            return string.Empty;
        }

        static string GetAbuse(string Whois)
        {
            string Pattern = @"(.*):\s+(\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b)";
            Match ServerMatch = Regex.Match(Whois, Pattern);

            if (ServerMatch.Success)
            {
              //  EmailList.Add(ServerMatch.Groups[1].Value, ServerMatch.Groups[2].Value);
                return ServerMatch.Groups[2].Value;
            }
            else
                return string.Empty;

        }

        
    }
}