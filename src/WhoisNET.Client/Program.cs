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
var options = Tokenizer.Tokenize("--no-recursion 1.1.1.1");

Dictionary<QueryOptions, object> queryOptions = [];

switch (options)
{
    case var o when o.ContainsKey(OptionEnum.query):
        queryOptions.Add(QueryOptions.query, o[OptionEnum.query]);
        break;
    case var o when o.ContainsKey(OptionEnum.host):
        queryOptions.Add(QueryOptions.host, o[OptionEnum.host]);
        break;
    case var o when o.ContainsKey(OptionEnum.port):
        queryOptions.Add(QueryOptions.port, o[OptionEnum.port]);
        break;
    case var o when o.ContainsKey(OptionEnum.debug):
        queryOptions.Add(QueryOptions.debug, o[OptionEnum.debug]);
        break;
    case var o when o.ContainsKey(OptionEnum.verbose):
        queryOptions.Add(QueryOptions.verbose, o[OptionEnum.verbose]);
        break;
    case var o when o.ContainsKey(OptionEnum.no_recursion):
        queryOptions.Add(QueryOptions.no_recursion, o[OptionEnum.no_recursion]);
        break;
    default:
        Console.WriteLine("error: no query specified.");
        break;
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