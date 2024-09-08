namespace WhoisNET.Client.CmdOptions
{
    public class OptionAttribute : IOptionAttribute
    {
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public string? ExpectedValue { get; set; }
        public string? Description { get; set; }
        public OptionEnum OptionName { get; set; }
        public bool IsFlag { get; set; }
        public bool UnknownOption { get; set; }
        public Type? ValueType { get; set; }
        public object? Value { get; set; }
    }
}
