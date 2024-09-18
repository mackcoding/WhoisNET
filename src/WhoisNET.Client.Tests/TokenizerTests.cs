using WhoisNET.Client.CmdOptions;

namespace WhoisNET.Client.Tests
{
    [TestFixture]
    public class TokenizerTests
    {
        [Test]
        public void TestBasicOptions()
        {
            var input = "--host example.com --port 80 query.com";
            var result = Tokenizer.Tokenize(input);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Has.Count.EqualTo(3));
                Assert.That(result, Contains.Key(OptionEnum.host));
                Assert.That(result[OptionEnum.host], Is.EqualTo("example.com"));
                Assert.That(result, Contains.Key(OptionEnum.port));
                Assert.That(result[OptionEnum.port], Is.EqualTo(80));
                Assert.That(result, Contains.Key(OptionEnum.query));
                Assert.That(result[OptionEnum.query], Is.EqualTo("query.com"));
            });
        }

        [Test]
        public void TestShortOptions()
        {
            var input = "-h example.com -p 80 query.com";
            var result = Tokenizer.Tokenize(input);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Has.Count.EqualTo(3));
                Assert.That(result, Contains.Key(OptionEnum.host));
                Assert.That(result[OptionEnum.host], Is.EqualTo("example.com"));
                Assert.That(result, Contains.Key(OptionEnum.port));
                Assert.That(result[OptionEnum.port], Is.EqualTo(80));
                Assert.That(result, Contains.Key(OptionEnum.query));
                Assert.That(result[OptionEnum.query], Is.EqualTo("query.com"));
            });
        }

        [Test]
        public void TestFlagOptions()
        {
            var input = "--debug --no-recursion query.com";
            var result = Tokenizer.Tokenize(input);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Has.Count.EqualTo(3));
                Assert.That(result, Contains.Key(OptionEnum.debug));
                Assert.That(result[OptionEnum.debug], Is.EqualTo(true));
                Assert.That(result, Contains.Key(OptionEnum.no_recursion));
                Assert.That(result[OptionEnum.no_recursion], Is.EqualTo(true));
                Assert.That(result, Contains.Key(OptionEnum.query));
                Assert.That(result[OptionEnum.query], Is.EqualTo("query.com"));
            });
        }

        [Test]
        public void TestMixedOptions()
        {
            var input = "-h example.com --port 80 -d --no-recursion query.com";
            var result = Tokenizer.Tokenize(input);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Has.Count.EqualTo(5));
                Assert.That(result, Contains.Key(OptionEnum.host));
                Assert.That(result[OptionEnum.host], Is.EqualTo("example.com"));
                Assert.That(result, Contains.Key(OptionEnum.port));
                Assert.That(result[OptionEnum.port], Is.EqualTo(80));
                Assert.That(result, Contains.Key(OptionEnum.debug));
                Assert.That(result[OptionEnum.debug], Is.EqualTo(true));
                Assert.That(result, Contains.Key(OptionEnum.no_recursion));
                Assert.That(result[OptionEnum.no_recursion], Is.EqualTo(true));
                Assert.That(result, Contains.Key(OptionEnum.query));
                Assert.That(result[OptionEnum.query], Is.EqualTo("query.com"));
            });
        }


        [Test]
        public void TestHelpOption()
        {
            var input = "--help";
            var result = Tokenizer.Tokenize(input);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.Empty);
            });
        }



        [Test]
        public void TestInvalidOption()
        {
            var input = "--invalid option";
            Assert.Throws<InvalidOperationException>(() => Tokenizer.Tokenize(input));
        }

        [Test]
        public void TestEmptyInput()
        {
            var input = "";
            Assert.Throws<InvalidOperationException>(() => Tokenizer.Tokenize(input));

        }

        [Test]
        public void TestQueryOnly()
        {
            var input = "query.com";
            var result = Tokenizer.Tokenize(input);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Has.Count.EqualTo(1));
                Assert.That(result, Contains.Key(OptionEnum.query));
                Assert.That(result[OptionEnum.query], Is.EqualTo("query.com"));
            });
        }
    }
}
