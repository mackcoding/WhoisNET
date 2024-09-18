namespace WhoisNET.Client.CmdOptions
{
    public static class Options
    {
        private static readonly Dictionary<OptionEnum, IOptionAttribute> _options = new()
        {
            [OptionEnum.host] = new OptionAttribute
            {
                Name = "host",
                Description = "Specifies which whois server to query.",
                ExpectedValue = "HOST",
                ShortName = "h",
                ValueType = typeof(string),
                OptionName = OptionEnum.host,
            },
            [OptionEnum.port] = new OptionAttribute
            {
                Name = "port",
                Description = "Specifies which server port to query.",
                ExpectedValue = "PORT",
                ShortName = "p",
                ValueType = typeof(int),
                OptionName = OptionEnum.port
            },
            [OptionEnum.debug] = new OptionAttribute
            {
                Name = "debug",
                Description = "Displays debug output",
                IsFlag = true,
                ShortName = "d",
                OptionName = OptionEnum.debug
            },
            [OptionEnum.no_recursion] = new OptionAttribute
            {
                Name = "no-recursion",
                Description = "Disables recursion from registry to registrar servers",
                IsFlag = true,
                ShortName = "r",
                OptionName = OptionEnum.no_recursion
            },
            [OptionEnum.help] = new OptionAttribute
            {
                Name = "help",
                Description = "Displays the command help",
                IsFlag = true,
                OptionName = OptionEnum.help
            }
        };

        public static IOptionAttribute? GetOption(string name)
        {
            name = name.Replace("-", "_");

            if (Enum.TryParse(name, true, out OptionEnum opt) && _options.TryGetValue(opt, out var attr))
                return attr;
            else if (name.Length == 1 && Enum.IsDefined(typeof(OptionEnum), (int)name[0]))
                return _options[(OptionEnum)(int)name[0]];

            return new OptionAttribute { UnknownOption = true, OptionName = OptionEnum.query };
        }

        public static void ShowHelp()
        {
            Console.WriteLine("Usage: whois [OPTION] OBJECT\n");

            int maxNameLength = _options.Values.Max(item => item.Name?.Length ?? 0);
            int maxShortNameLength = _options.Values.Max(item => item.ShortName?.Length ?? 0);
            int maxExpectedValueLength = _options.Values.Max(item => item.ExpectedValue?.Length ?? 0);

            foreach (var item in _options.Values)
            {
                string shortName = item.ShortName != null ? $"-{item.ShortName}" : "";
                string name = $"--{item.Name}";
                string expectedValue = item.ExpectedValue != null ? $"<{item.ExpectedValue}>" : "";

                string optionText = $"{shortName.PadRight(maxShortNameLength + 2)} {name.PadRight(maxNameLength + 2)} {expectedValue.PadRight(maxExpectedValueLength + 2)}";
                string descriptionText = item.Description ?? "";

                Console.WriteLine($"{optionText}  {descriptionText}");
            }
        }
    }
}