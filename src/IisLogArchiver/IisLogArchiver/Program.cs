using Autofac;
using CommandLine;
using CommandLine.Text;
using IisLogArchiver.Common.Extensions;
using IisLogArchiver.Interfaces;
using log4net;
using System;
using System.Diagnostics;

namespace IisLogArchiver
{
    internal class Program
    {
        private static ILog Log;

        private static void Main(string[] args)
        {
            Log4NetExtension.InitializeLoggersFromDefaultFile();
            Log = LogManager.GetLogger(typeof(Program));
            Log.Info("Logging init");

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
                Log.Info($"Exit program normally. Took: {watch.ElapsedFmtTimeHMS()}");
            }
        }
    }
}