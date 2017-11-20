using System;
using System.IO;
using IisLogArchiver.Core;
using IisLogArchiver.Interfaces;
using log4net;

namespace IisLogArchiver.FileHandling
{
    public class ArchiveNameProvider : IArchiveNameProvider
    {
        private readonly IFileProvider _fileProvider;
        private readonly IEnvironmentProvider _environmentProvider;
        private static readonly ILog Log = LogManager.GetLogger(typeof(ArchiveNameProvider));

        public ArchiveNameProvider(IFileProvider fileProvider, IEnvironmentProvider environmentProvider)
        {
            _fileProvider = fileProvider;
            _environmentProvider = environmentProvider;
        }
        public string DecideArchiveName(Archive archive, DateTime time)
        {
            var machinename = _environmentProvider.MachineName;
            var archiveName = $"{machinename}_{archive.ArchiveName}_{time:yyMMdd}.7z";
            var originalPath = Path.Combine(archive.ArchivePath, archiveName);
            if (!_fileProvider.Exists(originalPath))
            {
                Log.Info($"ArchivePath selected. path: {originalPath}");
                return originalPath;
            }
            Log.Warn($"Archive already exists for path: {originalPath}");

            //already exists, getting new name
            var iteration = 0;
            var alternativePath = string.Empty;
            do
            {
                var alternatveArchiveName = $"{machinename}_{archive.ArchiveName}_{time:yyMMdd}_{++iteration}.7z";
                alternativePath = Path.Combine(archive.ArchivePath, alternatveArchiveName);

                if (iteration > 1000)
                    throw new Exception("Failed to establish archive name.");
            } while (_fileProvider.Exists(alternativePath));
            Log.Info($"AlternativePath selected. path: {alternativePath}");
            return alternativePath;
        }
    }
}