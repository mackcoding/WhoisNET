using System;
using System.Net.Sockets;


namespace WhoisNET
{
    public static class Whois
    {
        public static void Query()
        {
            // whois.iana.org


        }

        private static void QueryRecursive(string Query, List<string> Servers, int Port, int Timeout = 500, int Retries = 10)
        {
            var Count = 0;
            var RawResp = string.Empty;

            while (string.IsNullOrEmpty(RawResp) && Count < Retries) {
              //  RawResp = 
            }

        }

        private static string RawQuery()
        {
            return string.Empty;
        }
    }
} 