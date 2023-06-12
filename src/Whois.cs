using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WhoisNET.Network;

namespace WhoisNET
{
    public static class Whois
    {

        #region Sync
        public static string Query(string Address)
        {
            string WhoisServer = GetWhoisServer(Address);
            string Response = string.Empty;

            Address = Utilities.GetDomain(Address.Replace("http://", "").Replace("https://", ""));

            using (var Connection = new TcpConnection(WhoisServer))
            {
                Connection.Connect();
                Connection.Send($"{GetQueryCmd(WhoisServer)} {Address}");
                Response = Connection.Receive();
            }

            return Response;
        }

        public static string GetWhoisServer(string Address)
        {
            var Tld = Utilities.GetTLD(Address);

            string Response = string.Empty;

            using (var Connection = new TcpConnection("whois.iana.org"))
            {
                Connection.Connect();
                Connection.Send(Tld);
                Response = Connection.Receive();
            }

            return Response;
        }
        #endregion

        #region Async
        public static async Task<string> QueryAsync(string Address)
        {
            string WhoisServer = await GetWhoisServerAsync(Address);
            string Response = string.Empty;

            Address = Utilities.GetDomain(Address.Replace("http://", "").Replace("https://", ""));

            using (var Connection = new TcpConnection(WhoisServer))
            {
                await Connection.ConnectAsync();
                await Connection.SendAsync($"{GetQueryCmd(WhoisServer)} {Address}");
                Response = await Connection.ReceiveAsync();
            }

            var Test = new WhoisResult(Response);


            Console.WriteLine($"{Test.GetValue(WhoisTokens.Comment)}");
            return Response;
        }

        public static async Task<string> GetWhoisServerAsync(string Address)
        {
            var Tld = Utilities.GetTLD(Address);

            string Response = string.Empty;

            using (var Connection = new TcpConnection("whois.iana.org"))
            {
                await Connection.ConnectAsync();
                await Connection.SendAsync(Tld);
                Response = await Connection.ReceiveAsync();
            }

            var Tokens = new WhoisResult(Response);


            return Tokens.GetValue(WhoisTokens.Refer);
        }
        #endregion

        #region Helpers
        public static string GetQueryCmd(string Server)
        {
            return Server switch
            {
                "whois.internic.net" or "whois.verisign-grs.com" => $"domain ",
                // This fixes the 'Query term are ambiguous' message when querying arin. 
                "whois.arin.net" => $"n +",
                _ => $"",// Remove the "domain" command from other servers
            };
        }
        #endregion

    }
}
