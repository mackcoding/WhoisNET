
using WhoisNET.Network;

namespace UnitTests
{
    public class QueryTests
    {
        [Fact]
        public void QueryGoogle()
        {
            string Data = QueryTools.GetWhois("https://google.com");

            Assert.Contains("Name Server: NS2.GOOGLE.COM", Data);
        }
    }
}