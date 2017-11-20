using System.Collections.Generic;

namespace IisLogArchiver.Interfaces
{
    public interface IFileBatchProvider
    {
        IEnumerable<IEnumerable<string>> Batch(IEnumerable<string> archiveFilesToCompress);
    }
}