namespace WhoisNET.Tests
{
    [Parallelizable(ParallelScope.All)]
    public class UtilityTest
    {
        #region GetTLD
        [TestCase("http://www.example.com", "com")]
        [TestCase("https://subdomain.example.co.uk", "co.uk")]
        [TestCase("example.org", "org")]
        [TestCase("test.blogspot.com", "com")]
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

        #region GetReferral
        [Test]
        public void GetReferral_SuccessfulMatch_ReturnsFormattedString()
        {
            var data = "ReferralServer: rwhois://example.com:80";
            var expected = "example.com:80";
            var result = Utilities.GetReferral(data);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetReferral_NoMatch_ReturnsEmptyString()
        {
            var data = "No referral information";
            var result = Utilities.GetReferral(data);
            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void GetReferral_MissingPort_ReturnsDefaultPort()
        {
            var data = "ReferralServer: rwhois://example.com";
            var expected = "example.com:43";
            var result = Utilities.GetReferral(data);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetReferral_MultipleMatches_ReturnsFirstOne()
        {
            var data = "ReferralServer: rwhois://a.com:80\nReferralServer: rwhois://b.com:443";
            var expected = "a.com:80";
            var result = Utilities.GetReferral(data);
            Assert.That(result, Is.EqualTo(expected));
        }
        #endregion
    }
}