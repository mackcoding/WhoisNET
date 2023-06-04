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
            Console.WriteLine(QueryTools.GetWhois("8.8.8.8"));
        }
        
    }
}