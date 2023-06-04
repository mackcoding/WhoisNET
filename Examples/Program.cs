using WhoisNET;
using System.Net;
using System.Net.Sockets;
using System.IO;
using WhoisNET.Network;
using System.Text.RegularExpressions;

namespace Whois.NET_Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var Lines = File.ReadAllLines("domains.txt");
            List<string> Result = new List<string>();

            //foreach (var Line in Lines)
           // {
            //    Console.WriteLine($"{Line}: {GetAbuse(QueryTools.GetWhois(Line))}");
           // }

            Console.WriteLine(QueryTools.GetWhois("8.8.8.8"));
            //Console.WriteLine(QueryTools.GetWhois("https://google.com"));
            //Console.WriteLine(QueryTools.GetWhois("generates.pw"));
            //Console.WriteLine(QueryTools.GetWhois("sre.google"));
            Console.ReadLine();
        }

        static string GetAbuse(string Whois)
        {
            string Pattern = @".* Abuse .* ([\w.-]+@[\w.-]+\.\w+)";
            Match ServerMatch = Regex.Match(Whois, Pattern);

            if (ServerMatch.Success)
                return ServerMatch.Groups[1].Value;
            else
                return string.Empty;

        }

        
    }
}