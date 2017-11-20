using System;
using System.IO;
using log4net;
using log4net.Config;

namespace IisLogArchiver.Common.Extensions
{
    public static class Log4NetExtension
    {
        private static ILog Log;
        public static void InitializeLoggersFromDefaultFile()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var configFile = Path.Combine(path, "log4net.config");
            InitializeLoggers(new FileInfo(configFile));
        }

        public static void InitializeLoggers(FileInfo file)
        {
            if (LogManager.GetCurrentLoggers().Length == 0)
                XmlConfigurator.Configure(file);
            Log = LogManager.GetLogger(typeof(Log4NetExtension));
            Log.Info("Logging init");
        }
    }
}