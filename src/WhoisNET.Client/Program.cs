using System.Diagnostics;
using WhoisNET;
using WhoisNET.Client.CmdOptions;
using WhoisNET.Enums;
using WhoisNET.Parser;

string text = @"% IANA WHOIS server
% for more information on IANA, visit http://www.iana.org
% This query returned 1 object

refer:        whois.arin.net

inetnum:      128.0.0.0 - 128.255.255.255
organisation: Administered by ARIN
status:       LEGACY

whois:        whois.arin.net

changed:      1993-05
source:       IANA";





string text2 =@"%rwhois V-1.5:003eff:00 rwhois.centrilogic.com (by Network Solutions, Inc. V-1.5.9.5)
%referral rwhois://root.rwhois.net:4321/auth-area=.
%ok";

Stopwatch sw = new Stopwatch();

sw.Start();
{
    ReferralParser.GetReferral(text2);
}
sw.Stop();

Console.WriteLine($"Execution Time: {sw.ElapsedMilliseconds} ms");

sw.Start();
{
    ReferralParser.GetReferral(text);
}
sw.Stop();

Console.WriteLine($"Execution Time: {sw.ElapsedMilliseconds} ms");

sw.Start();
{
    Utilities.GetReferral(text);
}
sw.Stop();

Console.WriteLine($"Execution Time: {sw.ElapsedMilliseconds} ms");

sw.Start();
{
    Utilities.GetReferral(text2);
}
sw.Stop();

Console.WriteLine($"Execution Time: {sw.ElapsedMilliseconds} ms");



try
{
    var options = Tokenizer.Tokenize("--verbose 128.14.219.125");
    //var options = Tokenizer.Tokenize(string.Join(' ', args));

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
