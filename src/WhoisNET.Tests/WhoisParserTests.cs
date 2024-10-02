using WhoisNET.Parser;

namespace WhoisNET.Tests
{
    [Parallelizable(ParallelScope.All)]
    public class WhoisParserTests
    {
        [Test]
        [TestCase("google.com", "Domain Name", "google.com")]
        [TestCase("example.com", "Registry Domain ID", "2336799_DOMAIN_COM-VRSN")]
        [TestCase("mackcoding.com", "Registrar IANA ID", "1910")]
        [TestCase("generates.pw", "Registrar Abuse Contact Email", "urgent@key-systems.net")]
        [TestCase("doesntexistatall111111.com", "comment", "No match for")]
        [TestCase("128.14.219.215", "NetHandle", "NET-128-14-219-0-1")]
        [TestCase("8.8.8.8", "OrgTechPhone", "+1-650-253-0000")]
        [TestCase("1.1.1.1", "nic-hdl", "DKDI10-AP")]
        public async Task Tokenize_ShouldReturnExpectedResult(string domain, string searchString, string expectedResult)
        {
            var query = await Whois.QueryAsync(domain);
            var whoisParser = new WhoisParser();

            whoisParser.Tokenize(query);

            var result = whoisParser.FindValue(searchString.ToLowerInvariant());

            Assert.Multiple(() =>
            {
                Assert.That(result.TryGetValue(searchString, out string? value), Is.True, $"Search string '{searchString}' not found.");
                Assert.That(value?.ToLower(), Does.Contain(expectedResult.ToLower()), $"Expected '{expectedResult}' to be a substring of '{value}'");
            });
        }

        [Test]
        [TestCase("google.com", 16)]
        [TestCase("example.com", 17)]
        [TestCase("mackcoding.com", 17)]
        [TestCase("generates.pw", 20)]
        [TestCase("doesntexistatall111111.com", 1)]
        [TestCase("128.14.219.215", 34)]
        [TestCase("8.8.8.8", 29)]
        [TestCase("1.1.1.1", 33)]
        public async Task Tokenize_ShouldReturnExpectedCount(string domain, int count)
        {
            var query = await Whois.QueryAsync(domain);
            var whoisParser = new WhoisParser();

            var result = whoisParser.Tokenize(query);

            Assert.That(result, Has.Count.EqualTo(count));
        }


    }
}
