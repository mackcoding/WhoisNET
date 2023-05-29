using WhoisNET;
using System.Net;
using System.Net.Sockets;
using System.IO;
using WhoisNET.Network;

namespace Whois.NET_Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Debug.DebugLogging = true;
            Console.WriteLine(QueryTools.GetWhois("thomasmack.dev"));
            Console.WriteLine(QueryTools.GetWhois("https://google.com"));
            Console.WriteLine(QueryTools.GetWhois("generates.pw"));
            Console.WriteLine(QueryTools.GetWhois("sre.google"));
            Console.ReadLine();
        }
    }
}