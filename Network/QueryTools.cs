using System.Text.RegularExpressions;


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

            string Pattern = @"whois:\s+(\S+)";
            Match ServerMatch = Regex.Match(Response, Pattern);

            if (ServerMatch.Success)
                return ServerMatch.Groups[1].Value;
            else
                return string.Empty;

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

        public static string GetQueryCmd(string Server)
        {
            switch (Server)
            {
                case "whois.internic.net":
                case "whois.verisign-grs.com":
                    return $"domain ";
                case "whois.arin.net": // This fixes the 'Query term are ambiguous' message when querying arin. 
                    return $"n + ";
                default:
                    // Remove the "domain" command from other servers
                    return $"";
            }
        }


    }
}
