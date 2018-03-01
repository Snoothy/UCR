using System;

namespace HidWizards.UCR.Core.Utilities
{
    public static class Logger
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private enum LogLevel{ Trace, Debug, Info, Warn, Error, Fatal }

        public static void Trace(string message, Exception e = null)
        {
            Log(LogLevel.Trace, message, e);
        }

        public static void Debug(string message, Exception e = null)
        {
            Log(LogLevel.Debug, message, e);
        }

        public static void Info(string message, Exception e = null)
        {
            Log(LogLevel.Info, message, e);
        }

        public static void Warn(string message, Exception e = null)
        {
            Log(LogLevel.Warn, message, e);
        }

        public static void Error(string message, Exception e)
        {
            Log(LogLevel.Error, message, e);
        }

        public static void Fatal(string message, Exception e)
        {
            Log(LogLevel.Fatal, message, e);
        }

        private static void Log(LogLevel logLevel, string message, Exception e)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    logger.Trace(e, message);
                    break;
                case LogLevel.Debug:
                    logger.Debug(e, message);
                    break;
                case LogLevel.Info:
                    logger.Info(e, message);
                    break;
                case LogLevel.Warn:
                    logger.Warn(e, message);
                    break;
                case LogLevel.Error:
                    logger.Error(e, message);
                    break;
                case LogLevel.Fatal:
                    logger.Fatal(e, message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }
    }
}
