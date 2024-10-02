using System.Text;
using System.Text.RegularExpressions;

namespace WhoisNET.Parser
{
    public partial class WhoisParser
    {
        private readonly Dictionary<string, string> tokens = new(StringComparer.OrdinalIgnoreCase);

        public Dictionary<string, string> Tokenize(string data)
        {
            tokens.Clear();
            string? line;

            using StringReader reader = new(data);

            while ((line = reader.ReadLine()) != null)
            {
                bool isComment = LineIsComment(line);
                bool isKey = true,
                    isValue = false;

                StringBuilder key = new(),
                    value = new();

                if (!isComment)
                {
                    for (int i = 0; i < line.Length; i++)
                    {
                        char c = line[i];

                        switch (c)
                        {
                            case ':':
                                isKey = false;
                                isValue = true;
                                break;
                        }

                        if (isKey && c != ':')
                            key.Append(c);
                        else if (isValue && c!= ':')
                            value.Append(c);
                    }

                    AddToken(key.ToString(), value.ToString(), tokens);
                } else
                {
                    AddToken("comment", line, tokens);
                }
            }
            return tokens;
        }

        private static void AddToken(string key, string value, Dictionary<string, string> lines)
        {
            key = key.Trim();
            value = value.Trim();

            if (lines.TryAdd(key, value) == false)
                lines[key] = $"{lines[key]}{Environment.NewLine}{value}";
        }

        private static bool LineIsComment(string line)
        {
            if (string.IsNullOrEmpty(line) ||
                    CharGroupMatch(line, ['#', '%', '>', '\t']) ||
                    CharGroupMatch(line, ['<'], false))
                return true;

            if (HasColonWithMaxSpaces(line))
                return true;

            if (!HasColon(line))
                return true;

            return false;
        }

        private static bool HasColonWithMaxSpaces(string line)
        {
            var cleanLine = RemoveHttp(line);
            int colonIndex = cleanLine.IndexOf(':');

            if (colonIndex == -1)
                return false;

            int spaceCount = 0;
            for (int i = colonIndex + 1; i < cleanLine.Length; i++)
            {
                var test = PeekChar(line, i);

                if (cleanLine[i] == ' ' && PeekChar(line, i) != ' ')
                    spaceCount++;

                if (spaceCount > 6)
                    return true;
            }

            return false;
        }

        private static char PeekChar(string line, int index)
        {
            return index < line.Length - 1 ? line[index + 1] : '\0';
        }

        private static bool CharGroupMatch(string line, char[] target, bool? firstChar = true)
        {
            return line.Length > 0 && Array.Exists(target, c => c == line[0]);
        }

        private static bool HasColon(string line)
        {
            var cleanLine = RemoveHttp(line);
            return HasChar(cleanLine, ':');
        }


        private static bool HasChar(string line, char target)
        {
            foreach (char c in line)
            {
                if (c == target)
                    return true;
            }
            return false;
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

        private static string RemoveHttp(string line)
        {
            return RemoveHttpsRegex().Replace(line, "").Trim();
        }

        // todo: very simple regex, not sure if we should eliminate or not.
        [GeneratedRegex("https?://")]
        private static partial Regex RemoveHttpsRegex();
    }
}
