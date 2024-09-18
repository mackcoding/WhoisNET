﻿using WhoisNET;
using WhoisNET.Client.CmdOptions;
using WhoisNET.Enums;
using WhoisNET.Parser;

var options = Tokenizer.Tokenize("204.2.29.65");

Dictionary<QueryOptions, object> queryOptions = [];

var optionMapping = new Dictionary<OptionEnum, QueryOptions>
{
    { OptionEnum.query, QueryOptions.query },
    { OptionEnum.host, QueryOptions.host },
    { OptionEnum.port, QueryOptions.port },
    { OptionEnum.debug, QueryOptions.debug },
    { OptionEnum.verbose, QueryOptions.verbose },
    { OptionEnum.no_recursion, QueryOptions.no_recursion }
};


foreach (var option in options)
{
    if (optionMapping.TryGetValue(option.Key, out var queryOption))
    {
        queryOptions.Add(queryOption, option.Value);
    }
}

if (!queryOptions.ContainsKey(QueryOptions.query))
{
    Console.WriteLine("error: no query specified.");
}

var result = await Whois.QueryAsync(queryOptions);


var parsedResult = WhoisParser.Tokenize(result);

Console.WriteLine($"{result}");
