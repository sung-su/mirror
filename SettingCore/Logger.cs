using System;
using System.Runtime.CompilerServices;

namespace SettingCore
{
    public static class Logger
    {
        private const string LogTag = "SettingCS";
        private const string PerfTag = "perf";

        public static void Verbose(string message, [CallerFilePath] string file = "", [CallerMemberName] string func = "", [CallerLineNumber] int line = 0)
        {
            Tizen.Log.Verbose(LogTag, message, file, func, line);
        }

        public static void Debug(string message, [CallerFilePath] string file = "", [CallerMemberName] string func = "", [CallerLineNumber] int line = 0)
        {
            Tizen.Log.Debug(LogTag, message, file, func, line);
        }

        public static void Warn(string message, [CallerFilePath] string file = "", [CallerMemberName] string func = "", [CallerLineNumber] int line = 0)
        {
            Tizen.Log.Warn(LogTag, message, file, func, line);
        }

        public static void Error(string message, [CallerFilePath] string file = "", [CallerMemberName] string func = "", [CallerLineNumber] int line = 0)
        {
            Tizen.Log.Error(LogTag, message, file, func, line);
        }

        public static void Performance(string message, [CallerFilePath] string file = "", [CallerMemberName] string func = "", [CallerLineNumber] int line = 0)
        {
            String timeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            Tizen.Log.Debug(PerfTag, $"{message} : {timeStamp}", file, func, line);
        }
    }
}
