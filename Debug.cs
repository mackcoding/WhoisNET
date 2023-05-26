using System.Diagnostics;
using System.Reflection;

namespace WhoisNET
{
    public static class Debug
    {
        public static bool DebugLogging = true;

        /// <summary>
        /// Writes to the console with severity default (OK)
        /// </summary>
        /// <param name="msg"></param>
        public static void Write(string msg)
        {
            GetCallingMethod(out string _name, out string _class);
            Write(WriteSeverity.OK, msg, _name, _class, true, true, true);
        }

        /// <summary>
        /// Writes to the console with severity Notice (OK)
        /// </summary>
        /// <param name="msg"></param>
        public static void Warn(string msg)
        {
            GetCallingMethod(out string _name, out string _class);
            Write(WriteSeverity.Notice, msg, _name, _class, true, true, true);
        }


        /// <summary>
        /// Writes to the console with severity Error (OK)
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(string msg)
        {
            GetCallingMethod(out string _name, out string _class);
            Write(WriteSeverity.Error, msg, _name, _class, true, true, true);
        }

        /// <summary>
        /// Writes to console without any severity or coloring
        /// </summary>
        /// <param name="msg"></param>
        public static void PlainLine(string msg)
        {
            GetCallingMethod(out string _name, out string _class);
            Write(WriteSeverity.OK, msg, _name, _class, false, false, true);
        }

        /// <summary>
        /// Writes to console without any severity or coloring
        /// </summary>
        /// <param name="msg"></param>
        public static void Plain(string msg)
        {
            GetCallingMethod(out string _name, out string _class);
            Write(WriteSeverity.OK, msg, _name, _class, false, false, false);
        }

        /// <summary>
        /// Writes a response to a console command (yellow)
        /// </summary>
        /// <param name="msg"></param>
        public static void ConsoleMsg(string msg)
        {
            GetCallingMethod(out string _name, out string _class);
            Write(WriteSeverity.Warning, msg, _name, _class, false, true, false);
        }





        /// <summary>
        /// Writes to the console
        /// </summary>
        /// <param name="Severity">Severity - Affects if the data gets written to syslogs and console colors</param>
        /// <param name="Msg">Msg</param>
        /// <param name="bWithClassAndMethod">Includes the calling class method and class name</param>
        /// <param name="bWithColor">Writes with color formatting</param>
        public static void Write(WriteSeverity Severity, string Msg, string Name, string Class, bool bWithClassAndMethod, bool bWithColor, bool bNewLine)
        {
            if (!DebugLogging) return;

            switch (Severity)
            {
                case WriteSeverity.OK:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;
                case WriteSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;
                case WriteSeverity.Critical:
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    break;
                case WriteSeverity.Failure:
                    break;
                case WriteSeverity.Notice:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;
                case WriteSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;

            }

            if (!bWithColor)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.BackgroundColor = ConsoleColor.Black;
            }

            if (bWithClassAndMethod)
                Console.WriteLine($"{Class}.{Name}: {Msg}");
            else
                Console.WriteLine($"{Msg}");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
        }


        private static void GetCallingMethod(out string MethodName, out string MethodClass)
        {
            StackFrame? Frame = new(5);
            var Method = Frame.GetMethod();
            if (Method != null)
            {
                MethodName = Method.Name;
                if (Method.DeclaringType != null)
                {
                    MethodClass = Method.DeclaringType.Name;
                }
                else { MethodClass = "unknown"; }
            }
            else
            {
                MethodName = "unknown";
                MethodClass = "unknown";
            }

        }

        public static void DrawTextProgressBar(string stepDescription, int progress, int total)
        {
            int totalChunks = 30;

            if (total == 0)
                return;

            //draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = totalChunks + 1;
            Console.Write("]"); //end
            Console.CursorLeft = 1;

            double pctComplete = Convert.ToDouble(progress) / total;
            int numChunksComplete = Convert.ToInt16(totalChunks * pctComplete);

            //draw completed chunks
            Console.BackgroundColor = ConsoleColor.Green;
            Console.Write("".PadRight(numChunksComplete));

            //draw incomplete chunks
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write("".PadRight(totalChunks - numChunksComplete));

            //draw totals
            Console.CursorLeft = totalChunks + 5;
            Console.BackgroundColor = ConsoleColor.Black;

            string output = progress.ToString() + " of " + total.ToString();
            Console.Write(output.PadRight(15) + stepDescription); //pad the output so when changing from 3 to 4 digits we avoid text shifting
        }

    }

    /// <summary>
    /// Handles write severity
    /// </summary>
    public enum WriteSeverity
    {
        OK,
        Notice,
        Warning,
        Critical,
        Error,
        Failure
    }
}

