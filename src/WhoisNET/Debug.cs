using System.Runtime.CompilerServices;

namespace WhoisNET
{
    /// <summary>
    /// Writes debugging to the console.
    /// </summary>
    public static class Debug
    {
        /// <summary>
        /// Defines the default level of debugging to use.
        /// </summary>
        private static LogLevel _logLevel = LogLevel.Off;

        /// <summary>
        /// Writes to the console with specified message.
        /// </summary>
        /// <param name="msg">Message to write</param>
        /// <param name="method">(optional) Source method</param>
        /// <param name="color">(optional) Color of console text</param>
        public static void Write(string msg, [CallerMemberName] string? method = null, ConsoleColor? color = null)
        {
            if (_logLevel == LogLevel.Off)
                return;

            Console.ResetColor();

            if (color is not null)
                Console.ForegroundColor = color.GetValueOrDefault();
            else 
                Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine($"({DateTime.Now}) [{method}]: {msg}");
            Console.ResetColor();
        }

        /// <summary>
        /// Writes debug message to the console with specified message.
        /// </summary>
        /// <param name="msg">Message to write</param>
        /// <param name="method">(optional) Source method</param>
        public static void WriteDebug(string msg, [CallerMemberName] string? method = null)
        {
            if (_logLevel != LogLevel.Debug)
                return;

            Write(msg, method, ConsoleColor.Yellow);
        }

        /// <summary>
        /// Writes debug message to the console with specified message.
        /// </summary>
        /// <param name="msg">Message to write</param>
        /// <param name="method">(optional) Source method</param>
        public static void WriteVerbose(string msg, [CallerMemberName] string? method = null)
        {
            if (_logLevel != LogLevel.Verbose)
                return;

            Write(msg, method, ConsoleColor.Magenta);
        }


        /// <summary>
        /// Writes exception to the console with specified message.
        /// </summary>
        /// <param name="msg">Message to write</param>
        /// <param name="method">(optional) Source method</param>
        public static void WriteException(string msg, [CallerMemberName] string? method = null)
        {
            if (_logLevel != LogLevel.Exception && _logLevel != LogLevel.Debug)
                return;

            Write(msg, method, ConsoleColor.DarkRed);
        }

        /// <summary>
        /// Throws an exception.
        /// </summary>
        /// <param name="msg">Message to write</param>
        /// <param name="method">(optional) Source method</param>
        /// <param name="exception">(optional) The thrown exception</param>
        public static void ThrowException(string msg, [CallerMemberName] string? method = null, Exception? exception = null)
        {
            WriteException(msg, method);
            if (exception is null) 
                throw new Exception(msg);

            throw exception;
        }

        /// <summary>
        /// Sets the default logging level. 
        /// </summary>
        public static LogLevel SetLogLevel
        {
            set
            {
                _logLevel = value;
            }
        }


    }



    /// <summary>
    /// Defines the log level.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Logging is off.
        /// </summary>
        Off = 0,
        /// <summary>
        /// Only log exceptions
        /// </summary>
        Exception = 1,
        /// <summary>
        /// Log debug messages
        /// </summary>
        Debug = 2,
        /// <summary>
        /// Log verbose messages
        /// </summary>
        Verbose = 3
          

    }
}

