using System;
using System.IO;
using System.Threading;
using IisLogArchiver.Interfaces;
using log4net;

namespace IisLogArchiver.FileHandling
{
    public class DirectoryDeleter : IDirectoryDeleter
    {
        private const int retryMaxCount = 120;
        private const int waitBeforeRetryMs = 1000;
        private readonly ILog _log = LogManager.GetLogger(typeof(DirectoryDeleter));
        private int retryCount;

        public bool EmptyDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                _log.Error($"path does not exist. Path: {directoryPath}");
                throw new ArgumentException($"path does not exist. Path: {directoryPath}");
            }
            try
            {
                TryEmptyDirectory(directoryPath);
            }
            catch (Exception e)
            {
                _log.Error(e);
                retryCount++;
                _log.Error(
                    $"Exception occured while emptying path {directoryPath}. Retry {retryCount} out of {retryMaxCount}");
                if (retryCount <= retryMaxCount)
                {
                    Thread.Sleep(waitBeforeRetryMs);
                    return EmptyDirectory(directoryPath);
                }
            }

            var isEmpty = Directory.GetFiles(directoryPath).Length == 0 &&
                          Directory.GetDirectories(directoryPath).Length == 0;

            return isEmpty;
        }

        private void TryEmptyDirectory(string target)
        {
            var files = Directory.GetFiles(target);
            var dirs = Directory.GetDirectories(target);
            TryDeleteFiles(files);

            foreach (var dir in dirs)
                TryDeleteDirectory(dir);
        }

        private void TryDeleteDirectory(string target)
        {
            var files = Directory.GetFiles(target);
            var dirs = Directory.GetDirectories(target);

            TryDeleteFiles(files);

            foreach (var dir in dirs)
                TryDeleteDirectory(dir);

            Directory.Delete(target, false);
        }

        private void TryDeleteFiles(string[] files)
        {
            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }
        }
    }
}