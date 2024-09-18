using WhoisNET;
using WhoisNET.Client.CmdOptions;
using WhoisNET.Enums;


/*
if (args.Length == 0)
{
    Options.ShowHelp();
    return;
}

var options = Tokenizer.Tokenize(string.Join(' ', args));*/
var options = Tokenizer.Tokenize("1.1.1.1");

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

// Handle the case where no query is specified
if (!queryOptions.ContainsKey(QueryOptions.query))
{
    Console.WriteLine("error: no query specified.");
}

var result = await Whois.QueryAsync(queryOptions);


Console.WriteLine($"{result}");

/*
try
{
    var options = Tokenizer.Tokenize(string.Join(' ', args));

    string? query = null;
    string? hostname = null;
    int? port = null;
    bool noRecursion = false;
    bool debug = false;

    var optionActions = new Dictionary<OptionEnum, Action<object>>
    {
        [OptionEnum.port] = v => port = int.TryParse(v?.ToString(), out int p) ? p : null,
        [OptionEnum.host] = v => hostname = v?.ToString(),
        [OptionEnum.query] = v => query = v?.ToString(),
        [OptionEnum.debug] = _ => debug = true,
        [OptionEnum.verbose] = _ => Console.WriteLine("Not yet implemented."),
        [OptionEnum.no_recursion] = _ => noRecursion = true
    };



    foreach (var (key, value) in options)
    {
        if (optionActions.TryGetValue(key, out var action))
            action(value);
    }

    if (debug)
    {
        Debug.SetLogLevel = LogLevel.Debug;
    }


    if (!string.IsNullOrEmpty(query))
    {
        var result = await Whois.QueryAsync(query ?? "", hostname, !noRecursion, queryPort: port ?? 43);
        Console.WriteLine($"{result}");
    }
}
catch (Exception err)
{
    Console.WriteLine($"error: {err.Message}");
}
*/