namespace WhoisNET.Tests
{
    [Parallelizable(ParallelScope.All)]
    public class QueryTests
    {
        [Test]
        [TestCase("google.com", "Creation Date: 1997-09-15T04:00:00Z")]
        [TestCase("example.com", "Creation Date: 1995-08-14T04:00:00Z")]
        [TestCase("mackcoding.com", "Creation Date: 2013-04-15T16:40:05Z")]
        [TestCase("generates.pw", "Creation Date: 2015-08-25T15:11:26.0Z")]
        [TestCase("doesntexistatall111111.com", "No match for \"DOESNTEXISTATALL111111.COM\".")]

        [TestCase("128.14.219.125", "OrgName:        Zenlayer Inc")]
        [TestCase("8.8.8.8", "OrgName:        Google LLC")]
        [TestCase("1.1.1.1", "descr:          APNIC and Cloudflare DNS Resolver project")]
        [TestCase("209.182.100.10", "NetName:        AS-SERVERION")]
        [TestCase("4.4.4.4", "OrgName:        Level 3 Parent, LLC")]
        [TestCase("2600::", "Sprint")]
        [TestCase("192.168.1.1", "remarks:      http://www.iana.org/go/rfc1918")]
        [TestCase("204.2.29.86", "network:City:Kawasaki, Kanagawa")]
        public async Task WhoisClientAsyncTest(string domain, string expectedSubstring)
        {
            string response = await Whois.QueryAsync(domain);

            Assert.That(response, 
                Does.Contain(expectedSubstring), 
                $"Response for {domain} does not contain expected substring: {expectedSubstring}");
        }
    }
}