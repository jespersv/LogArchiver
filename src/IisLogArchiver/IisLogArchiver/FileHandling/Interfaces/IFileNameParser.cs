using System;

namespace IisLogArchiver.Interfaces
{
    public interface IFileNameParser
    {
        bool TryParseDateFromString(string fileName, out DateTime fileDate);
    }
}