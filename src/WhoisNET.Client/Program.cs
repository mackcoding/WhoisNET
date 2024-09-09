using WhoisNET;
using WhoisNET.Client.CmdOptions;


// todo: make the whois library accept a dictionary of options instead
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
