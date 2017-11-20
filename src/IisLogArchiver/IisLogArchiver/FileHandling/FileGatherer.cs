using IisLogArchiver.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;

namespace IisLogArchiver.FileHandling
{
    public class FileGatherer : IFileGatherer
    {
        private readonly IDirectoryProvider _directoryProvider;
        private readonly IFileNameParser _fileNameParser;
        private static readonly ILog Log = LogManager.GetLogger(typeof(FileGatherer));

        public FileGatherer(IDirectoryProvider directoryProvider, IFileNameParser fileNameParser)
        {
            _directoryProvider = directoryProvider;
            _fileNameParser = fileNameParser;
        }
        public List<string> FilesOlderThan(string folderPath, DateTime date)
        {
            var files = _directoryProvider.GetFiles(folderPath);
            var dayBeforeDate = date.AddDays(-1);
            Log.Info($"Getting files for/before: {date:yyMMdd} on path: {folderPath}");

            var olderFilesFound = false;

            var result = new List<string>();
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);

                DateTime fileDate;
                if (!_fileNameParser.TryParseDateFromString(fileName, out fileDate))
                {
                    Log.Warn($"Files' date could not be determined base on name. Filename: {fileName}");
                    continue;
                }

                if (fileDate.Date <= date.Date)
                {
                    if (fileDate.Date <= dayBeforeDate.Date && !olderFilesFound)
                        olderFilesFound = true;
                    result.Add(file);
                }
            }

            if (olderFilesFound)
                Log.Info($"Archive includes older files than yesterday.");
            return result;
        }
    }
}