using System.Diagnostics;
using WhoisNET;
using WhoisNET.Client.CmdOptions;
using WhoisNET.Enums;
using WhoisNET.Parser;


try
{
    //var options = Tokenizer.Tokenize("--verbose 128.14.219.125");
    var options = Tokenizer.Tokenize(string.Join(' ', args));

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


    Console.WriteLine($"{result}");
}
catch (Exception err)
{
    Console.WriteLine($"{err.Message}");
}
