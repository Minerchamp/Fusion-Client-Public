using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Console = FCConsole.Console;
using MelonLoader;

namespace FusionClient.ConsoleUtils
{
    [Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
    public class ModConsole
    {
        private static bool HasRenamedOldLogFile = false;

        private static bool HasInitiated = false;

        private static string LogsPath
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, $"External Logs\\{BuildInfo.Name}");
            }
        }

        private static string LatestLogFile
        {
            get
            {
                return Path.Combine(LogsPath, BuildInfo.Name + "_Latest.log");
            }
        }

        private static int LogInt = 0;

        private static int GetCurrentInt()
        {
            
            return LogInt++;
        }

        public static string GetNewFileName()
        {
            var result = GetCurrentInt();
            var newfilename = BuildInfo.Name + "-Log-" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + " (" + result + ").log";
            if (!File.Exists(Path.Combine(LogsPath, newfilename)))
            {
                return Path.Combine(LogsPath, newfilename);
            }
            else
            {
                return GetNewFileName();
            }
        }

        public static void ReplaceOldLatestFile()
        {
            var tmp = GetNewFileName();
            if (File.Exists(LatestLogFile))
            {
                File.Move(LatestLogFile, tmp);
            }
        }

        public static void InitLogsSetup()
        {
            if (!HasInitiated)
            {
                if (!Directory.Exists(LogsPath))
                {
                    Directory.CreateDirectory(LogsPath);
                }
                if (!HasRenamedOldLogFile)
                {
                    ReplaceOldLatestFile();
                    HasRenamedOldLogFile = true;
                }
                HasInitiated = true;
            }
        }

        public static void Write(string msg)
        {
            if (!HasInitiated)
            {
                InitLogsSetup();
            }

            ConsoleMutex.WaitOne();
            File.AppendAllText(LatestLogFile, msg);
            ConsoleMutex.ReleaseMutex();
        }

        public enum LogTypes
        {
            LOG,
            WARNING,
            ERROR,
            DEBUG_LOG,
            DEBUG_WARNING,
            DEBUG_ERROR
        }

        public static void LogExc<T>(T e)
        {
            Exception((e as System.Exception), LogTypes.LOG);
        }

        public static void WarningExc<T>(T e)
        {
            Exception((e as System.Exception), LogTypes.WARNING);
        }

        public static void ErrorExc<T>(T e)
        {
            Exception((e as System.Exception), LogTypes.ERROR);
        }

        public static void DebugLogExc<T>(T e)
        {
            Exception((e as System.Exception), LogTypes.DEBUG_LOG);
        }

        public static void DebugWarningExc<T>(T e)
        {
            Exception((e as System.Exception), LogTypes.DEBUG_WARNING);
        }

        public static void DebugErrorExc<T>(T e)
        {
            Exception((e as System.Exception), LogTypes.DEBUG_ERROR);
        }

        public static void Log(string msg, Color? textcolor = null)
        {
            if (textcolor == null)
            {
                textcolor = Color.PapayaWhip;
            }
            PrintTags(LogTypes.LOG);
            PrintLine(msg, textcolor.Value);
        }

        public static void Warning(string msg, Color? textcolor = null)
        {
            if (textcolor == null)
            {
                textcolor = Color.Yellow;
            }
            PrintTags(LogTypes.WARNING);
            PrintLine(msg, textcolor.Value);
        }

        public static void Error(string msg, Color? textcolor = null)
        {
            if (textcolor == null)
            {
                textcolor = Color.Red;
            }
            PrintTags(LogTypes.ERROR);
            PrintLine(msg, textcolor.Value);
        }

        public static void Exception<T>(T e, LogTypes logType = LogTypes.LOG, Color? color = null)
        {
            if (logType == LogTypes.DEBUG_LOG || logType == LogTypes.DEBUG_WARNING )
            {
                return;
            }
            if (color == null)
            {
                color = Color.Red;
            }

            PrintTags(logType);

            PrintLine(); // Basically an easy way to newline
            PrintLine($"Exception Message: {(e as System.Exception).Message}", color.Value);
            PrintLine($"Exception StackTrace: {(e as System.Exception).StackTrace}", color.Value);
            PrintLine($"Exception TargetSite: {(e as System.Exception).TargetSite}");
            PrintLine($"Exception Source: {(e as System.Exception).Source}");
        }

        public static void PrintLine(string msg = "", Color? color = null)
        {
            Console.Write(msg + Environment.NewLine, color.Value);
            Task.Run(() => { Write(msg + Environment.NewLine); });
        }

        private static void PrintTags(LogTypes logType = LogTypes.LOG)
        {
            PrintTimestamp();
            PrintModStamp();

            switch (logType)
            {
                case LogTypes.LOG:
                    PrintLogTag();
                    break;

                case LogTypes.WARNING:
                    PrintWarningTag();
                    break;

                case LogTypes.ERROR:
                    PrintErrorTag();
                    break;

                case LogTypes.DEBUG_LOG:
                    PrintDebugLogTag();
                    break;

                case LogTypes.DEBUG_WARNING:
                    PrintDebugWarningTag();
                    break;

                case LogTypes.DEBUG_ERROR:
                    PrintDebugErrorTag();
                    break;
            }
        }

        private static void PrintLogTag()
        {
            Console.Write("[", Color.White);
            Console.Write("LOG", Color.Aqua);
            Console.Write("]: ", Color.White);
            Task.Run(() => { Write("[LOG]: "); });
        }

        private static void PrintWarningTag()
        {
            Console.Write("[", Color.White);
            Console.Write("WARNING", Color.Orange);
            Console.Write("]: ", Color.White);
            Task.Run(() => { Write("[WARNING]: "); });
        }

        private static void PrintErrorTag()
        {
            Console.Write("[", Color.White);
            Console.Write("ERROR", Color.Red);
            Console.Write("]: ", Color.White);
            Task.Run(() => { Write("[ERROR]: "); });
        }

        private static void PrintDebugLogTag()
        {
            Console.Write("[", Color.White);
            Console.Write("DEBUG LOG", Color.Aquamarine);
            Console.Write("]: ", Color.White);
            Task.Run(() => { Write("[DEBUG LOG]: "); });
        }

        private static void PrintDebugWarningTag()
        {
            Console.Write("[", Color.White);
            Console.Write("DEBUG WARNING", Color.Orange);
            Console.Write("]: ", Color.White);
            Task.Run(() => { Write("[DEBUG WARNING]: "); });
        }

        private static void PrintDebugErrorTag()
        {
            Console.Write("[", Color.White);
            Console.Write("DEBUG ERROR", Color.Red);
            Console.Write("]: ", Color.White);
            Task.Run(() => { Write("[DEBUG ERROR]: "); });
        }

        private static void PrintModStamp()
        {
            Console.Write("[", Color.White);
            Console.Write(BuildInfo.Name, Color.Gold);
            Console.Write("] ", Color.White);
            Task.Run(() => { Write($"[{BuildInfo.Name}] "); });
        }

        private static void PrintTimestamp()
        {
            string time = DateTime.Now.ToString("HH:mm:ss.fff");
            Console.Write("[", Color.White);
            Console.Write(time, Color.LightGreen);
            Console.Write("] ", Color.White);
            Task.Run(() => { Write($"[{time}] "); });
        }

        private static System.Threading.Mutex ConsoleMutex = new System.Threading.Mutex();
    }
}