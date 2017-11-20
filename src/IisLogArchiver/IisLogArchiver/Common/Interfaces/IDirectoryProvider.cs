using System.Collections.Generic;

namespace IisLogArchiver.Interfaces
{
    public interface IDirectoryProvider
    {
        IEnumerable<string> GetFiles(string folderPath);
        bool Exists(string folderPath);
        void CreateDirectory(string folderPath);
    }
}