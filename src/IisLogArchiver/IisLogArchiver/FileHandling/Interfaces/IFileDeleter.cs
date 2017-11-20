using System.Collections.Generic;

namespace IisLogArchiver.Interfaces
{
    public interface IFileDeleter
    {
        bool TryDeleteFiles(IEnumerable<string> files, bool deleteFiles);
        void TryDeleteFile(string file, bool delete);
    }
}