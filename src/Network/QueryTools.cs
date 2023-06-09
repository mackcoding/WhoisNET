﻿using System.Text.RegularExpressions;


namespace WhoisNET.Network
{
    public static class QueryTools
    {
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

           return GetReferOrWhoisServer(Response);
        }

        public static string GetWhois(string Address)
        {
            string Response = string.Empty;
            string Server = GetWhoisServer(Address);

            Address = Utilities.GetDomain(Address.Replace("http://", "").Replace("https://", ""));

            using (var Connection = new TcpConnection(Server))
            {
                Connection.Connect();
                Connection.Send($"{GetQueryCmd(Server)} {Address}");
                Response = Connection.Receive();
            }

            return Response;
        }

        public static string GetReferOrWhoisServer(string Whois)
        {
            string Result = string.Empty;

            List<string> ListOfTests = new()
            {
                "whois:\\s+(\\S+)",
                "refer:\\s+(\\S+)",
            };

            foreach (var Pattern in ListOfTests)
            {
                Match ServerMatch = Regex.Match(Whois, Pattern);

                if (ServerMatch.Success)
                    Result=ServerMatch.Groups[1].Value;
            }

            return Result;
        }

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


    }
}
