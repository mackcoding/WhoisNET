namespace WhoisNET.Tests
{
    [Parallelizable(ParallelScope.All)]
    public class UtilityTest
    {
        #region GetTLD
        [TestCase("http://www.example.com", "com")]
        [TestCase("https://subdomain.example.co.uk", "co.uk")]
        [TestCase("example.org", "org")]
        [TestCase("test.blogspot.com", "blogspot.com")]
        [TestCase("www.gov.uk", "gov.uk")]
        public async Task GetTLD_ValidUrls_ReturnsCorrectTLD(string url, string expectedTLD)
        {
            string result = await Utilities.GetTLD(url);

            Assert.That(result, Is.EqualTo(expectedTLD));
        }

        [Test]
        public void GetTLD_InvalidUrl_ThrowsArgumentException()
        {
            string invalidUrl = "not a valid url";
            Assert.ThrowsAsync<ArgumentException>(async () => await Utilities.GetTLD(invalidUrl));
        }
        #endregion


    }
}