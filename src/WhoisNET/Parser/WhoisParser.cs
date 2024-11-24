using System.Text;
using System.Text.RegularExpressions;

namespace WhoisNET.Parser
{
    /// <summary>
    /// Parser that tokenizes the whois data.
    /// </summary>
    public partial class WhoisParser
    {
        private readonly Dictionary<string, string> tokens = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Tokenizes the whois data into a token dictionary.
        /// </summary>
        /// <param name="data">Whois text to tokenize</param>
        /// <returns>Dictionary with data tokenized into key:value.</returns>
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

        /// <summary>
        /// Adds a new token to the internal Dictionary
        /// </summary>
        /// <param name="key">Key - used to identify item</param>
        /// <param name="value">Value of the key</param>
        /// <param name="lines">Reference to the token dictionary</param>
        private static void AddToken(string key, string value, Dictionary<string, string> lines)
        {
            key = key.Trim();
            value = value.Trim();

            if (lines.TryAdd(key, value) == false)
                lines[key] = $"{lines[key]}{Environment.NewLine}{value}";
        }

        /// <summary>
        /// Makes an educated determination on if the line is a comment
        /// </summary>
        /// <param name="line">String of the text to check</param>
        /// <returns>true = line is a comment, otherwise false</returns>
        private static bool LineIsComment(string line)
        {
            if (string.IsNullOrEmpty(line) ||
                    CharGroupMatch(line, ['#', '%', '>', '\t']) ||
                    CharGroupMatch(line, ['<']))
                return true;

            if (HasColonWithMaxSpaces(line))
                return true;

            if (!HasColon(line))
                return true;

            return false;
        }

        /// <summary>
        /// Searches a line for a colon
        /// </summary>
        /// <param name="line">String of the text to check</param>
        /// <returns>true = found a colon, otherwise false</returns>
        private static bool HasColonWithMaxSpaces(string line)
        {
            var cleanLine = RemoveHttp(line);
            int colonIndex = cleanLine.IndexOf(':');

            if (colonIndex == -1)
                return false;

            int spaceCount = 0;
            for (int i = colonIndex + 1; i < cleanLine.Length; i++)
            {
                //var test = PeekChar(line, i);

                if (cleanLine[i] == ' ' && PeekChar(line, i) != ' ')
                    spaceCount++;

                if (spaceCount > 6)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the next char. 
        /// </summary>
        /// <param name="line">String of the text to check</param>
        /// <param name="index">Index of the peek char</param>
        /// <returns>Next char based off the index</returns>
        private static char PeekChar(string line, int index)
        {
            return index < line.Length - 1 ? line[index + 1] : '\0';
        }

        /// <summary>
        /// Runs a char match using a group of chars
        /// </summary>
        /// <param name="line">String of the text to check</param>
        /// <param name="target">Array of targets to find</param>
        /// <returns>true if a match is found, otherwise false</returns>
        private static bool CharGroupMatch(string line, char[] target)
        {
            return line.Length > 0 && Array.Exists(target, c => c == line[0]);
        }

        /// <summary>
        /// Checks if a line has a colon
        /// </summary>
        /// <param name="line">String of the text to check</param>
        /// <returns>true = colon found, otherwise false</returns>
        private static bool HasColon(string line)
        {
            var cleanLine = RemoveHttp(line);
            return HasChar(cleanLine, ':');
        }


        /// <summary>
        /// Checks if the line has specified char.
        /// </summary>
        /// <param name="line">String of the text to check</param>
        /// <param name="target">Target letter to find</param>
        /// <returns>true = found match, otherwise false.</returns>
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
