using IisLogArchiver.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IisLogArchiver.FileHandling
{
    public class FileDeleter : IFileDeleter
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(FileDeleter));

        private readonly IFileProvider _fileProvider;
        private readonly IThreadProvider _threadProvider;

        private const int retryMaxCount = 120;
        private const int waitBeforeRetryMs = 1000;
        private int retryCount;

        public FileDeleter(IFileProvider fileProvider, IThreadProvider threadProvider)
        {
            _fileProvider = fileProvider;
            _threadProvider = threadProvider;
        }

        public bool TryDeleteFiles(IEnumerable<string> files, bool deleteFiles)
        {
            var filesListed = files.ToList();
            var filteredFiled = filesListed.Where(f => !_fileProvider.IsFileLocked(f)).ToList();

            if (filteredFiled.Count != filesListed.Count)
                _log.Info($"Some files where locked. Amount: {filesListed.Count - filteredFiled.Count}");

            foreach (var file in filteredFiled)
                try
                {
                    _fileProvider.DeleteFile(file, deleteFiles);
                }
                catch (Exception e)
                {
                    _log.Error(e);
                    retryCount++;
                    _log.Error($"Exception occured while deleting filePath: {file}. Retry {retryCount} out of {retryMaxCount}");
                    if (retryCount <= retryMaxCount)
                    {
                        _threadProvider.Sleep(waitBeforeRetryMs);
                        return TryDeleteFiles(filteredFiled, deleteFiles);
                    }
                }

            var filesDeleted = filteredFiled.All(f => !_fileProvider.Exists(f));
            _log.Info(filesDeleted ? $"Files were successfully deleted" : $"Some files were not deleted");

            return filesDeleted;
        }

        public void TryDeleteFile(string file, bool delete)
        {
            if (!_fileProvider.Exists(file))
            {
                _log.Warn("File does not exist, cannot delete");
                return;
            }
            if (_fileProvider.IsFileLocked(file))
            {
                _log.Warn("File is locked, cannot delete");
                return;
            }

            try
            {
                _fileProvider.DeleteFile(file, delete);
            }
            catch (Exception e)
            {
                _log.Error($"Error during deletion of file. {file}");
                _log.Error(e.StackTrace);
            }
        }
    }
}