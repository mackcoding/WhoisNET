using System.Text;

namespace WhoisNET.Parser
{
    public class WhoisParserOld
    {
        private readonly List<string> _reserved = ["notice", "terms of use"];
        private readonly Dictionary<string, string> tokens = new(StringComparer.OrdinalIgnoreCase);


        /// <summary>
        /// Tokenizes the whois data into a dictionary.
        /// </summary>
        /// <param name="data">Whois data to tokenize</param>
        /// <returns>Dictionary of whois data</returns>
        public Dictionary<string, string> Tokenize3(string data)
        {
            string? line;
            tokens.Clear();
            bool lineIsComment = false;

            using StringReader reader = new(data);

            while ((line = reader.ReadLine()) != null)
            {
                var key = new StringBuilder();
                var value = new StringBuilder();

                bool isKey = HasChar(line, ':'),
                    isValue = false,
                    isComment = false,
                    lastLineIsComment = false,
                    lineIsUrl = LineContainsUrl(line);


                if (lastLineIsComment)
                    isComment = true;

                for (int current = 0; current < line.Length; current++)
                {
                    char c = line[current];
                    char firstChar = line[0];
                    char previous = current > 0 ? line[current - 1] : '\0';
                    char next = current < line.Length - 1 ? line[current + 1] : '\0';

                    switch (firstChar)
                    {
                        case '#' or '>' or '%' or '\t':
                            isComment = true;
                            isKey = false;
                            isValue = true;
                            lineIsComment = true;
                            break;
                    }

                    switch (c)
                    {
                        case ':' when isKey && !isValue && !isComment:
                            isKey = false;
                            isValue = true;
                            isComment = false;
                            lineIsComment = false;
                            break;
                    }

                    if (lineIsUrl && !isKey || lineIsComment)
                    {
                        isKey = false;
                        isValue = true;
                    }

                    if (isKey && !HasChar(line, ':'))
                    {
                        isKey = false;
                        isValue = true;
                    }

                    if (c != ':')
                    {
                        if (isKey && !isComment)
                            key.Append(c);
                        else if (isValue)
                            value.Append(c);
                        else if (isComment)
                            value.Append(c);
                    }
                }

                isKey = true;
                isValue = false;
                lastLineIsComment = isComment;
                isComment = false;

                if (key.Length == 0 || isComment)
                    key.Append("info");

                if (key is not null && value is not null)
                    AddLine(key.ToString().Trim(), value.ToString().Trim(), tokens);

                Console.WriteLine($"key: {key}\nvalue:{value}\n\n");
            }

            return tokens;
        }

        public Dictionary<string, string> Tokenize(string data)
        {
            string? line;
            tokens.Clear();
            bool lineIsComment = true,
                lastLineWasComment = false;

            using StringReader reader = new(data);

            while ((line = reader.ReadLine()) != null)
            {
                var LineIsComment = IsLineComment(line);

                Console.WriteLine($"LineIsComment: {LineIsComment}\nline: {line}\n");

                for (int current = 0; current < line.Length; current++)
                {
                }
            }


            return tokens;
        }

        /* Overview
         * Determines if the line is a comment. 
         * If the line starts with #, >, %, or \t, but does not have a :, it's likely a comment
         * If there is a : but also contains a link, it's not a comment
         * If there there is a link but no :, it is a comment
         * If there is a : but the following text has more than 5 spaces, it's likely a comment
        */
        private static bool IsLineComment(string line)
        {
            if (string.IsNullOrEmpty(line) || FirstCharGroupMatch(line, 
                ['#', '>', '%', '\t']) || PeekChar(line, line.Length - 1) == '<')
                return true;

            bool hasColon = false;
            int spaceCount = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == ':')
                {
                    hasColon = true;
                    continue;
                }

                if (hasColon && line[i] == ' ')
                    spaceCount++;

                if (spaceCount >= 5)
                    return true;
            }

            return false;
        }

        private static bool LineHasColon(string line)
        {
            bool foundHttp = false;
            char prev = new();

            for (int i = 1; i < line.Length; i++)
            {
                if (prev == 'h' && line[i] == 't')
                    foundHttp = true;

                if (line[i] == ':' && !foundHttp)
                    return true;

                prev = line[i];
            }

            return false;
        }

        private static int LineSpaceCount(string line)
        {
            int count = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == ' ')
                    count++;
            }

            return count;
        }

        private static bool FirstCharGroupMatch(string line, char[] target)
        {
            return line.Length > 0 && Array.Exists(target, c => c == line[0]);
        }

        private static bool FirstCharMatch(string line, char target)
        {
            return line[0] == target;
        }

        // Comments: 
        //  case '#' or '>' or '%' or '\t':
        // # % 


        /// <summary>
        /// Tokenizes the whois data into a dictionary.
        /// </summary>
        /// <param name="data">Whois data to tokenize</param>
        /// <returns>Dictionary of whois data</returns>
        public Dictionary<string, string> Tokenize2(string data)
        {
            string? line;
            tokens.Clear();

            using StringReader reader = new(data);

            while ((line = reader.ReadLine()) != null)
            {
                var key = new StringBuilder();
                var value = new StringBuilder();

                bool isKey = true,
                    isValue = false,
                    containsUrl = LineContainsUrl(line),
                    isComment = false,
                    isInfo = false;

                // Check if the line starts with a comment or info marker
                if (line.TrimStart().StartsWith("#") || line.TrimStart().StartsWith(">"))
                {
                    isComment = true;
                    isInfo = true;
                }

                for (int current = 0; current < line.Length; current++)
                {
                    char c = line[current];

                    switch (c)
                    {
                        case ' ' when !isKey && !isValue && !isComment && !isInfo:
                            if (PeekChar(line, current) == ' ' && !containsUrl)
                                isKey = true;
                            break;
                        case ':' when isKey && !isValue && !isComment && !isInfo:
                            isKey = false;
                            isValue = true;
                            break;
                        default:
                            if (!isComment && !isInfo)
                            {
                                if (isKey && !isValue)
                                    key.Append(c);
                                else if (isValue)
                                    value.Append(c);
                            }
                            else
                            {
                                value.Append(c);
                            }
                            break;
                    }
                }

                if (isComment || isInfo)
                {
                    key.Append("info");
                }
                else if (key.Length == 0)
                {
                    key.Append("info");
                }

                if (key is not null && value is not null)
                    AddLine(key.ToString().Trim(), value.ToString().Trim(), tokens);
            }

            return tokens;
        }

        private static int CountChar(string line, char target)
        {
            int count = 0;
            for (int i = 1; i < line.Length; i++)
            {
                if (line[i] == target)
                    count++;
            }

            return count;
        }

        private static char PeekFirstChar(string line)
        {
            return line[0];
        }

        private static bool HasChar(string line, char target)
        {
            for (int i = 1; i < line.Length; i++)
            {
                if (line[i] == target)
                    return true;
            }
            return false;
        }

        private static char PeekChar(string line, int index)
        {
            return index < line.Length - 1 ? line[index + 1] : '\0';
        }

        private static char PeekPrevChar(string line, int index)
        {
            if (index - 1 < 0)
                return '\0';

            return line[index - 1];
        }

        private static bool LineContainsUrl(string line) =>
            line.Contains("http://", StringComparison.OrdinalIgnoreCase) ||
            line.Contains("https://", StringComparison.OrdinalIgnoreCase);


        private static void AddLine(string key, string value, Dictionary<string, string> lines)
        {
            if (lines.TryAdd(key, value) == false)
                lines[key] = $"{lines[key]}{Environment.NewLine}{value}";
        }

        /// <summary>
        /// Searches the tokenized data for a key, uses 'Contains' with 'StringComparison.OrdinalIgnoreCase'.
        /// </summary>
        /// <param name="search">The search term</param>
        /// <returns>Dictionary of matched results</returns>
        public Dictionary<string, string> FindValue(string search)
        {
            Dictionary<string, string> results = [];

            foreach (var item in tokens)
            {
                if (item.Key.Contains(search, StringComparison.OrdinalIgnoreCase))
                    results.TryAdd(item.Key, item.Value);
            }

            return results;
        }
    }
}
