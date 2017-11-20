using System;
using System.Collections.Generic;

namespace IisLogArchiver.Interfaces
{
    public interface IFileGatherer
    {
        List<string> FilesOlderThan(string folderPath, DateTime date);
    }
}