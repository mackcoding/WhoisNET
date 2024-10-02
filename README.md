# Project Goals

I'm developing this for two reasons:

1. I want to bring the 'whois' command to Windows.
2. I analyze spam using another application (this will also be open sourced) and automate reporting. I need a proper whois library for this.

# Status

I am actively working on it. It is not a full time project and is only updated when I have time.

# Commands
A basic tokenizer returns a list options. This will be expanded in the future.

Usage: whois [OPTION] OBJECT

```
-h  --host         <HOST>  Specifies which whois server to query.
-p  --port         <PORT>  Specifies which server port to query.
-d  --debug                Displays debug output
-r  --no-recursion         Disables recursion from registry to registrar servers
    --help                 Displays the command help
```

# Making available to Windows command/powershell

- Compile the application
- Update the `{PATH}` to where ever the compiled exe is
- Open powershell or command and type `whois --help`

```
[System.Environment]::SetEnvironmentVariable("Path", $env:Path + ";{PATH}", [System.EnvironmentVariableTarget]::Machine)
```

This adds the path to the environment variables.

# Crossplatform

Should be, haven't tested it yet!

# Contributing

Please! I'd love help on it. I'm learning as I go, if you want to contribute simply make a PR and after I review it, I'll approve it. Feel free to test and submit issues.

# Testing

Current testing is very limited. If you have IP addresses or domains you'd like me to add to the tests, let me know.

# Usage

I honestly don't care what you do with the code. If you want to reuse it, feel free. Please credit me if you can, or at least let me know what you are using it for.