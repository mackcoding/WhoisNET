using WhoisNET;

Debug.SetLogLevel = LogLevel.Debug;
Console.WriteLine($"{await Whois.QueryAsync("204.2.29.86")}");
