namespace WhoisNET.Client.CmdOptions
{
    /// <summary>
    /// Manages the Option attribute; inherits from IOptionAttribute
    /// </summary>
    public class OptionAttribute : IOptionAttribute
    {
        /// <summary>
        /// Name of the option
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Short name of the option
        /// </summary>
        public string? ShortName { get; set; }
        /// <summary>
        /// Expected Value of the option
        /// </summary>
        public string? ExpectedValue { get; set; }
        /// <summary>
        /// Description of the option
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Option name as an enum
        /// </summary>
        public OptionEnum OptionName { get; set; }
        /// <summary>
        /// Denotes if the option is a flag (true/false)
        /// </summary>
        public bool IsFlag { get; set; }
        /// <summary>
        /// Indicates if it's an unknown option
        /// </summary>
        public bool UnknownOption { get; set; }
        /// <summary>
        /// Type of the value
        /// </summary>
        public Type? ValueType { get; set; }
        /// <summary>
        /// Value of the option
        /// </summary>
        public object? Value { get; set; }
    }
}