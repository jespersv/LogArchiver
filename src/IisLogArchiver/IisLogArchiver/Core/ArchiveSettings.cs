using IisLogArchiver.Interfaces;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;

namespace IisLogArchiver.Core
{
    public class ArchiveSettings : BaseSettings, IArchiveSettings
    {
        public ArchiveSettings(ICommandLineOptions options)
        {
            var settingsPath = string.IsNullOrEmpty(options.ConfigPath) ? 
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "archives.json") : options.ConfigPath;
            var json = File.ReadAllText(settingsPath);
            SettingsRootObject = JsonConvert.DeserializeObject<ArchiveSettingsRootObject>(json);

            ArgumentLengthBeforePerCompress = int.Parse(GetFromAppConfig("ArgumentLengthBeforePerCompress", "30000"));
            CompressionPriority = (ProcessPriorityClass)Enum.Parse(typeof(ProcessPriorityClass), GetFromAppConfig("CompressionPriority", "BelowNormal"));
        }

        public ArchiveSettingsRootObject SettingsRootObject { get; }
        public int ArgumentLengthBeforePerCompress { get; private set; }
        public ProcessPriorityClass CompressionPriority { get; private set; }
    }

    public class ArchiveSettingsRootObject
    {
        public Archive[] Archives;
    }

    public class Archive
    {
        public string ArchiveName;
        public string ArchivePath;
        public bool DeleteArchivedFiles;
        public string FilePath;
    }
}