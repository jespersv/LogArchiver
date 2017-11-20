using IisLogArchiver.Core;
using IisLogArchiver.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IisLogArchiver
{
    public class Archiver : IArchiver
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Archiver));
        private readonly IArchiveNameProvider _archiveNameProvider;
        private readonly ICompressor _compressor;
        private readonly IFileDeleter _fileDeleter;
        private readonly IFileGatherer _fileGatherer;
        private readonly IDirectoryProvider _directoryProvider;

        private readonly IArchiveSettings _settings;
        private readonly ITimeProvider _timeProvider;
        private readonly IFileBatchProvider _fileBatchProvider;

        public Archiver(
            IArchiveSettings settings,
            ITimeProvider timeprovider,
            ICompressor compressor,
            IFileDeleter fileDeleter,
            IArchiveNameProvider archiveNameProvider,
            IFileGatherer fileGatherer,
            IDirectoryProvider directoryProvider,
            IFileBatchProvider fileBatchProvider
            )
        {
            _settings = settings;
            _timeProvider = timeprovider;
            _compressor = compressor;
            _fileDeleter = fileDeleter;
            _archiveNameProvider = archiveNameProvider;
            _fileGatherer = fileGatherer;
            _directoryProvider = directoryProvider;
            _fileBatchProvider = fileBatchProvider;
        }

        public void ProcessLogsToArchives(IEnumerable<Archive> archivesInput)
        {
            var archives = archivesInput.ToList();
            var yesterday = _timeProvider.Now.AddDays(-1);

            if (!archives.Any())
            {
                Log.Warn($"No archives to be processes found in configuration");
                return;
            }

            Log.Info($"Total amount of archives to process: {archives.Count}");
            var count = 0;
            foreach (var archive in archives)
            {
                try
                {
                    ProcessArchive(archive, yesterday);
                }
                catch (Exception e)
                {
                    Log.Error($"Archive {archive.ArchiveName} threw an error. Most likly skipped.");
                    Log.Error(e);
                }
                Log.Info($"Done archiving number {++count} out of {archives.Count}");
            }
        }

        private void ProcessArchive(Archive archive, DateTime yesterday)
        {
            //add in older than yesterdays .log files
            var archiveFilesToCompress = _fileGatherer.FilesOlderThan(archive.FilePath, yesterday);
            if (!archiveFilesToCompress.Any())
            {
                Log.Info($"No files found to be archived. FilePath: {archive.FilePath}");
                return;
            }

            var fileListToCompress = _fileBatchProvider.Batch(archiveFilesToCompress);

            foreach (var filesToCompress in fileListToCompress)
            {
                var filesTo = _archiveNameProvider.DecideArchiveName(archive, yesterday);
                var filesFrom = filesToCompress.ToArray();

                if (!_directoryProvider.Exists(archive.ArchivePath))
                {
                    Log.Warn($"Archive folder does not exist. Creating... {archive.ArchivePath}");
                    _directoryProvider.CreateDirectory(archive.ArchivePath);
                }

                var failed = _compressor.Compress(filesTo, filesFrom);
                //Deletes old logs if there were no error from the compression process
                if (!failed)
                    _fileDeleter.TryDeleteFiles(filesFrom, archive.DeleteArchivedFiles);
            }
        }
    }
}