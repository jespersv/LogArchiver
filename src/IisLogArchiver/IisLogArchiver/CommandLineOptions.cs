using CommandLine;

namespace IisLogArchiver
{
    public interface ICommandLineOptions
    {
        string ConfigPath { get; set; }
    }

    public class CommandLineOptions : ICommandLineOptions
    {
        [Option("configPath", Required = false, HelpText = "Specify the path for the archive file to use(json). None uses archives.json.")]
        public string ConfigPath { get; set; }
    }
}
