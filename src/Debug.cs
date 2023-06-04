using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Xml.Linq;

namespace WhoisNET
{
    public static class Debug
    {
        public static LogLevel LoggingLevel = LogLevel.Debug;

        public static void Write(string Message, [CallerMemberName] string MethodName = "")
        {
            if (LoggingLevel == LogLevel.Off) { return; }

            Write(LogLevel.Message, Message, MethodName);
        }

        public static void WriteError(string Message, [CallerMemberName] string MethodName = "")
        {
            if (LoggingLevel == LogLevel.Off) { return; }

            Write(LogLevel.Error, Message, MethodName);
        }

        public static void WriteDebug(string Message, [CallerMemberName]string MethodName = "")
        {
            if (LoggingLevel == LogLevel.Off) { return; }

            Write(LogLevel.Debug, Message, MethodName);
        }

        public static void Write(LogLevel Level, string Message, [CallerMemberName] string MethodName = "")
        {
            if (LoggingLevel == LogLevel.Off) { return; }

            switch (Level)
            {
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;
            }


            Console.WriteLine($"{GetCallingClass()}.{MethodName} ({DateTime.Now}): {Message}");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
           
        }

        private static string GetCallingClass() => new StackTrace()?.GetFrame(3)?.GetMethod()?.DeclaringType?.Name ?? "";

    }

    /// <summary>
    /// Defines the log level
    /// </summary>
    public enum LogLevel
    {  
        Off = 0,
        All,
        Message,
        Info,
        Debug,
        Error
    }
}

