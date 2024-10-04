namespace WhoisNET.Enums
{
    /// <summary>
    /// Defines valid options
    /// </summary>
    public enum QueryOptions
    {
        /// <summary>
        /// Whois Server hostname
        /// </summary>
        host,
        /// <summary>
        /// Whois Server port
        /// </summary>
        port,
        /// <summary>
        /// Debug Logging to console
        /// </summary>
        debug,
        /// <summary>
        /// Verbose Logging to console
        /// </summary>
        verbose,
        /// <summary>
        /// Disables recursion
        /// </summary>
        no_recursion,
        /// <summary>
        /// Whois query IP or Domain
        /// </summary>
        query
    }
}
