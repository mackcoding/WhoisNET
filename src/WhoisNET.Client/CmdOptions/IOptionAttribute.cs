namespace WhoisNET.Client.CmdOptions
{
    public interface IOptionAttribute
    {
        string? Name { get; set; }
        string? ShortName { get; set; }
        string? ExpectedValue { get; set; }
        string? Description { get; set; }
        OptionEnum OptionName { get; set; }
        bool IsFlag { get; set; }
        bool UnknownOption { get; set; }
        Type? ValueType { get; set; }
        object? Value { get; set; }

    }
}
