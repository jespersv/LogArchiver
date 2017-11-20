using IisLogArchiver.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace IisLogArchiver.Common
{
    public class DirectoryProvider : IDirectoryProvider
    {
        public IEnumerable<string> GetFiles(string folderPath)
        {
            return Directory.GetFiles(folderPath);
        }

        public bool Exists(string folderPath)
        {
            return Directory.Exists(folderPath);
        }

        public void CreateDirectory(string folderPath)
        {
            Directory.CreateDirectory(folderPath);
        }
    }
}