namespace WhoisNET.Client.CmdOptions
{
    public enum OptionEnum : int
    {
        host = (int)'h',
        port = (int)'p',
        debug = (int)'d',
        verbose = (int)'v',
        no_recursion = (int)'r',
        query,
        help
    }
}