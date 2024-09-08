using System.Text;
using System.Xml.Linq;

namespace WhoisNET.Client.CmdOptions
{
    public static class Tokenizer
    {
        readonly static Dictionary<OptionEnum, object> tokens = [];

        public static Dictionary<OptionEnum, object> Tokenize(string input)
                                             {
            var option = new StringBuilder();
            var value = new StringBuilder();
            bool isValue = false,
                isOption = false,
                isFlag = false;

            tokens.Clear();

            foreach (char c in input)
            {
                switch (c)
                {
                    case '-':
                        if (option.Length > 0 && value.Length > 0)
                        {
                            AddToken(option.ToString(), value);
                            option.Clear();
                            value.Clear();
                            isValue = false;
                        }
                        isOption = true;
                        break;

                    default:
                        if (char.IsWhiteSpace(c))
                        {
                            if (option.Length > 0 && value.Length > 0)
                            {
                                isFlag = AddToken(option.ToString(), value);
                                option.Clear();
                                value.Clear();
                                isValue = false;
                                isOption = false;
                            }

                            if (isOption)
                            {
                                isValue = true;
                                isOption = false;
                            }
                            else
                            {
                                isValue = false;
                            }
                        }
                        else if (isOption)
                        {
                            option.Append(c);
                        }
                        else if (isValue)
                        {
                            value.Append(c);
                        }
                        else if (option.Length == 0 && value.Length >= 0)
                        {
                            value.Append(c);
                        }
                        break;
                }
            }

            if (option.ToString().Equals("help", StringComparison.Ordinal))
            {
                Options.ShowHelp();
                return tokens;
            }

            if (option.Length > 0)
            {
                isFlag = AddToken(option.ToString(), value);

                if (isFlag)
                {
                    option.Clear();

                }
            }

            if ((option.Length == 0 && value.Length > 0) || isFlag)
                AddToken("query", value.ToString(), true);
            else
                throw new InvalidOperationException($"Query value is missing.");

            return tokens;
        }

        private static bool AddToken(string name, object value, bool isQuery = false)
        {
            var option = Options.GetOption(name) ?? throw new InvalidOperationException("error: option is null.");
            value = value is StringBuilder sb ? sb.ToString() : value;
            value = int.TryParse(value.ToString(), out int intresult) ? intresult : value;
            value = bool.TryParse(value.ToString(), out bool boolresult) ? boolresult : value;

            if (!isQuery && option.UnknownOption)
            {
                Options.ShowHelp();
                throw new InvalidOperationException($"Unrecognized option: '{name}'. Use --help to see available options.");
            }

            if (!option.IsFlag)
            {
                if (!isQuery)
                {
                    if (value.GetType() != option.ValueType)
                        throw new InvalidCastException($"Invalid value for option '{name}': " +
                            $"Expected {option.ValueType?.Name.ToLower()}, but received {value.GetType().Name.ToLower()}.");
                }

                tokens.Add(option.OptionName, value);
            }
            else
                tokens.Add(option.OptionName, true);

            return option.IsFlag;
        }
    }
}
