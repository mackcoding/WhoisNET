### Description

This is a .NET 7.0 class library that implements a WHOIS Library.

### Current Usage

WhoisNET will eventually get pushed to nuget, but during development, you must clone this repository and compile the project. There is nothing special needed to compile.

### Sample Code

The **async** methods are not yet available. To use the code in the current state:

```csharp
using WhoisNET;

static void Main(string[] args)
{
    Console.WriteLine(QueryTools.GetWhois("8.8.8.8"));
}
```
