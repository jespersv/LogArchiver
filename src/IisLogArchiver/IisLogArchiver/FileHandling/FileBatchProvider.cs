using IisLogArchiver.Interfaces;
using log4net;
using System.Collections.Generic;
using System.Linq;

namespace IisLogArchiver.FileHandling
{
    public class FileBatchProvider : IFileBatchProvider
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FileBatchProvider));
        private readonly int MaxLength;//processstartinfo can't handle more than 32k charlength

        public FileBatchProvider(IArchiveSettings archiveSettings)
        {
            MaxLength = archiveSettings.ArgumentLengthBeforePerCompress;
        }

        public IEnumerable<IEnumerable<string>> Batch(IEnumerable<string> archiveFilesToCompress)
        {
            var batches = new List<List<string>>();
            var batch = new List<string>();
            var count = 0;
            foreach (var file in archiveFilesToCompress)
            {
                if (count > MaxLength)
                {
                    Log.Warn($"Max count of characters reached for files. BatchSizes: {MaxLength} Batching...");
                    batches.Add(batch.ToList());
                    batch = new List<string>();
                    count = 0;
                }
                count += file.Length;
                batch.Add(file);
            }
            batches.Add(batch.ToList());
            Log.Info($"Batch count for this archive: {batches.Count}");

            return batches;
        }
    }
}