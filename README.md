# Project Goals

The primary goal of this project is to build a library that enables C# developers to query IP addresses and domains. Although still under heavy development, it aims to fully support querying against both RDAP and WHOIS. Currently, the data is converted into a Dictionary<key, value> format for use, such as retrieving abuse information. In the future, there will be an option to output data in JSON format.

This project consists of two main components:

- **Library**: The core of the project, which will eventually be released on NuGet. It is designed to be lightweight and fast, with minimal dependencies on external libraries.
- **Client**: Utilizes the library to bring the whois command to Windows, allowing users to execute the command from PowerShell or any command prompt. The project supports Windows and Linux, and likely macOS (though untested, as I do not use macOS). In theory, this library will run on any platform .NET 8.0 is supported on. 

This project is built from the ground up to ensure efficiency and cross-platform compatibility.

# Status

Active development!

# Commands
A basic tokenizer returns a list options. This will be expanded in the future.

Usage: whois [OPTION] OBJECT

```
-h  --host         <HOST>  Specifies which whois server to query.
-p  --port         <PORT>  Specifies which server port to query.
-d  --debug                Displays debug output
-d  --verbose              Displays verbose output
-r  --no-recursion         Disables recursion from registry to registrar servers
    --help                 Displays the command help
```

# Making available to Windows command/powershell

- Compile the application
- Update the `{PATH}` to whatever the compiled exe is
- Open powershell or command and type `whois --help`

```
[System.Environment]::SetEnvironmentVariable("Path", $env:Path + ";{PATH}", [System.EnvironmentVariableTarget]::Machine)
```

Adds the path to the environment variables.

# Crossplatform

Currently verified to work on Windows and Linux.

# Contributing

Please! I'd love help on it. I'm learning as I go, if you want to contribute simply make a PR and after I review it, I'll approve it. Feel free to test and submit issues.

# Testing

Current testing is very limited. If you have IP addresses or domains you'd like me to add to the tests, let me know.

# Usage

I honestly don't care what you do with the code. If you want to reuse it, feel free. Please credit me if you can, or at least let me know what you are using it for.
