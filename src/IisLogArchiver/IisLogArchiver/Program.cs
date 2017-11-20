using Autofac;
using CommandLine;
using CommandLine.Text;
using IisLogArchiver.Interfaces;
using log4net;
using log4net.Config;
using System;
using System.Diagnostics;
using System.IO;

namespace IisLogArchiver
{
    internal class Program
    {
        private static ILog Log;

        private static void Main(string[] args)
        {
            InitializeLogger();
            var options = ParseCommandlineArgs(args, "IisLogArchiver");

            try
            {
                DoArchiving(options);
            }
            catch (Exception e)
            {
                Log.Fatal("Unhandled global exception. Exception stacktrace:");
                Log.Fatal(e.Message);
                Log.Fatal(e.StackTrace);
                throw;
            }
        }

        private static ICommandLineOptions ParseCommandlineArgs(string[] args, string appName)
        {
            var options = new CommandLineOptions();
            var successfulParse = Parser.Default.ParseArguments(args, options);

            if (!successfulParse)
            {
                var help = HelpText.AutoBuild(options);
                help.Copyright = "Biltema";
                help.Heading = appName;

                Log.Fatal(help.ToString());
                Environment.Exit(0);
            }
            return options;
        }

        private static void DoArchiving(ICommandLineOptions commandlineoptions)
        {
            var containerBuilder = IisLogArchiverBootstrapper.GetContainerBuilder();
            containerBuilder.Register(p => commandlineoptions);
            var container = containerBuilder.Build();
            using (var scope = container.BeginLifetimeScope())
            {
                var watch = Stopwatch.StartNew();
                var archives = scope.Resolve<IArchiveSettings>().SettingsRootObject.Archives;
                scope.Resolve<IArchiver>().ProcessLogsToArchives(archives);
                watch.Stop();
                Log.Info($"Exit program normally. Took: {watch.Elapsed.Hours}h {watch.Elapsed.Minutes}m {watch.Elapsed.Seconds}s");
            }
        }

        private static void InitializeLogger()
        {
            if (LogManager.GetCurrentLoggers().Length == 0)
            {
                var path = AppDomain.CurrentDomain.BaseDirectory;
                var configFile = path + "log4net.config";
                XmlConfigurator.Configure(new FileInfo(configFile));
            }
            Log = LogManager.GetLogger(typeof(Program));
            Log.Info("Logging init");
        }
    }
}