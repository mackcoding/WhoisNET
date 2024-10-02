using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhoisNET.Parser
{
    public class WhoisParser
    {
        private static List<string> _reserved = ["notice", "terms of use"];

        public static Dictionary<string, string> Tokenize(string data)
        {
            var result = new Dictionary<string, string>();
            string? line;

            using StringReader reader = new(data);

            while ((line = reader.ReadLine()?.Trim()) != null)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                bool isComment = false,
                    isKey = true,
                    isValue = false,
                    isFirstValueChar = false,
                    keyHasWhitespace = false,
                    valueHasWhitespace = false;

                int keyWhiteSpaceCount = 0,
                    valueWhiteSpaceCount = 0;

                var key = new StringBuilder();
                var value = new StringBuilder();

                Console.WriteLine($"line: {line}");
                foreach (char c in line)
                {
                    switch (c)
                    {
                        case '#':
                            isKey = false;
                            isValue = false;
                            isComment = true;
                            break;
                        case ':' when isKey && !isValue && !keyHasWhitespace:
                            isKey = false;
                            isValue = true;
                            isFirstValueChar = true;
                            break;
                        case ' ' when isKey:
                            keyWhiteSpaceCount++;

                            if (keyWhiteSpaceCount > 3)
                                keyHasWhitespace = true;
                            break;
                        case ' ' when isValue:
                            valueWhiteSpaceCount++;

                            if (valueWhiteSpaceCount > 5)
                                valueHasWhitespace = true;
                            break;

                        case '\n':
                        case '\r':
                        case '\t':
                        case ' ':
                            break;
                    }

                    if (isKey)
                        key.Append(c);

                    if (isValue)
                    {
                        if (c is ':' && isFirstValueChar)
                        {
                            isFirstValueChar = false;
                            continue;
                        }

                        value.Append(c);
                    }

                    if (isComment)
                        break;
                }


                if (isKey && !isValue || keyHasWhitespace)
                    AddLine("info", key.ToString().Trim(), ref result);
                else if (_reserved.Contains(key.ToString().ToLower()))
                    AddLine("info", $"{key}: {value}\n", ref result);
                else if (valueHasWhitespace)
                    AddLine("info", $"{key} {value}\n", ref result);

                else if (isValue)
                    AddLine(key.ToString().Trim(), value.ToString().Trim(), ref result);
                else if (isComment)
                    AddLine("comment", line, ref result);


                //     AddLine(key.ToString().Trim(), value.ToString().Trim(), ref result);
                // key.Clear();
                // value.Clear();
            }

            return result;
        }

        private static void AddLine(string key, string value, ref Dictionary<string, string> lines)
        {
            if (lines.TryAdd(key, value) == false)
                lines[key] = $"{lines[key]}{Environment.NewLine}{value}";
        }

        // todo: going to rewrite to be similar to the Client's tokenizer. 
        public static Dictionary<string, string> Tokenizer(string whoisData)
        {
            var result = new Dictionary<string, string>();
            using (StringReader reader = new(whoisData))
            {
                string? line;
                string? currentKey = string.Empty;
                StringBuilder currentValue = new();

                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith('%') || line.StartsWith('#'))
                        continue;


                    int separatorIndex = line.IndexOf(':');

                    if (separatorIndex > 0)
                    {
                        if (currentKey != null)
                        {
                            result[currentKey] = currentValue.ToString().Trim();
                            currentValue.Clear();
                        }

                        currentKey = line[..separatorIndex].Trim();
                        string valuePart = line[(separatorIndex + 1)..].Trim();

                        currentValue.Append(valuePart);
                    }
                    else if (currentKey != null)
                    {
                        currentValue.AppendLine();
                        currentValue.Append(line.Trim());
                    }
                }

                if (currentKey != null)
                {
                    result[currentKey] = currentValue.ToString().Trim();
                }
            }
            return result;
        }
    }
}
