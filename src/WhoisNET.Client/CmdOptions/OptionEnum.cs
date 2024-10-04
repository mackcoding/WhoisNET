namespace WhoisNET.Client.CmdOptions
{
    /// <summary>
    /// Options used for command line.
    /// </summary>
    public enum OptionEnum : int
    {
        /// <summary>
        /// Hostname of the whois server
        /// </summary>
        host = (int)'h',
        /// <summary>
        /// Port of the whois server
        /// </summary>
        port = (int)'p',
        /// <summary>
        /// Debug mode
        /// </summary>
        debug = (int)'d',
        /// <summary>
        /// Verbose mode
        /// </summary>
        verbose = (int)'v',
        /// <summary>
        /// Disables recursion
        /// </summary>
        no_recursion = (int)'r',
        /// <summary>
        /// Domain or IP to query
        /// </summary>
        query,
        /// <summary>
        /// Help text
        /// </summary>
        help
    }
}