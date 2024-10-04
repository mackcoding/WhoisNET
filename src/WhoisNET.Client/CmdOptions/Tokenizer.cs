using System.Text;

namespace WhoisNET.Client.CmdOptions
{
    /// <summary>
    /// Handles converting command options to tokens for processing.
    /// </summary>
    public static class Tokenizer
    {
        readonly static Dictionary<OptionEnum, object> tokens = [];

        /// <summary>
        /// Tokenizes the input into valid options
        /// </summary>
        /// <param name="input">Text to tokenize</param>
        /// <returns>Dictionary of the tokenized options</returns>
        /// <exception cref="InvalidOperationException">Invalid option</exception>
        public static Dictionary<OptionEnum, object> Tokenize(string input)
        {
            var option = new StringBuilder();
            var value = new StringBuilder();
            bool isValue = false,
                isOption = false,
                isFlag = false;
            char previous = default;

            Debug.WriteVerbose($"Tokenize called with input: {input}");
            tokens.Clear();

            foreach (char c in input)
            {
                var isDashInToken = !(previous == default(char) || char.IsWhiteSpace(previous) || previous == '-');

                switch (c)
                {
                    case '-' when !isDashInToken:
                        if (option.Length > 0 && char.IsWhiteSpace(previous))
                        {
                            AddToken(option.ToString(), value);
                            option.Clear();
                            value.Clear();
                            isValue = false;
                        }

                        isOption = true;
                        break;
                    default:
                        if (c == '-' && isDashInToken)
                        {
                            option.Append(c);
                            isOption = true;
                        }
                        else if (char.IsWhiteSpace(c))
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

                previous = c;
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
            {
                Debug.WriteVerbose($"Added query token: {value}");
                AddToken("query", value.ToString(), true);
            }
            else
            {
                Debug.WriteVerbose($"Query value is missing");
                throw new InvalidOperationException($"Query value is missing.");
            }

            Debug.WriteVerbose($"Returning {tokens.Count} token(s)");
            return tokens;
        }

        /// <summary>
        /// Adds a new token to the internal dictionary.
        /// </summary>
        /// <param name="name">Key of the token</param>
        /// <param name="value">Value of the token</param>
        /// <param name="isQuery">if true, sets the key to query</param>
        /// <returns>true if successful; otherwise false.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        private static bool AddToken(string name, object value, bool isQuery = false)
        {
            var option = Options.GetOption(name) ?? throw new InvalidOperationException("error: option is null.");
            value = value is StringBuilder sb ? sb.ToString() : value;
            value = int.TryParse(value.ToString(), out int intresult) ? intresult : value;
            value = bool.TryParse(value.ToString(), out bool boolresult) ? boolresult : value;

            Debug.WriteVerbose($"{name}:{value} - isQuery: {isQuery}");

            if (!isQuery && option.UnknownOption)
            {
                Debug.WriteVerbose($"invalid option.");
                Options.ShowHelp();
                throw new InvalidOperationException($"\nUnrecognized option: '{name}'. Use --help to see available options.");
            }

            if (!option.IsFlag)
            {
                if (!isQuery)
                {
                    if (value.GetType() != option.ValueType)
                    {
                        Debug.WriteVerbose($"invalid option value: {name}");
                        throw new InvalidCastException($"\nInvalid value for option '{name}': " +
                            $"Expected {option.ValueType?.Name.ToLower()}, but received {value.GetType().Name.ToLower()}.");
                    }
                }

                Debug.WriteVerbose($"Added token {option.OptionName}:{value}");
                tokens.Add(option.OptionName, value);
            }
            else
            {
                Debug.WriteVerbose($"Added token: {option.OptionName}:true");
                tokens.Add(option.OptionName, true);
            }

            return option.IsFlag;
        }
    }
}
