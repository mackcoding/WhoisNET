namespace WhoisNET.Client.CmdOptions
{
    /// <summary>
    /// Interface that manages the tokenization of command line arguments.
    /// </summary>
    public interface IOptionAttribute
    {
        /// <summary>
        /// Name of the option
        /// </summary>
        string? Name { get; set; }
        /// <summary>
        /// Short name of the option
        /// </summary>
        string? ShortName { get; set; }
        /// <summary>
        /// Expected Value of the option
        /// </summary>
        string? ExpectedValue { get; set; }
        /// <summary>
        /// Description of the option
        /// </summary>
        string? Description { get; set; }
        /// <summary>
        /// Option name as an enum
        /// </summary>
        OptionEnum OptionName { get; set; }
        /// <summary>
        /// Denotes if the option is a flag (true/false)
        /// </summary>
        bool IsFlag { get; set; }
        /// <summary>
        /// Indicates if it's an unknown option
        /// </summary>
        bool UnknownOption { get; set; }
        /// <summary>
        /// Type of the value
        /// </summary>
        Type? ValueType { get; set; }
        /// <summary>
        /// Value of the option
        /// </summary>
        object? Value { get; set; }

    }
}
