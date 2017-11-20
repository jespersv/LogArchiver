using System;
using IisLogArchiver.Core;

namespace IisLogArchiver.Interfaces
{
    public interface IArchiveNameProvider
    {
        /// <summary>
        ///     Determines as name for the archive file
        /// </summary>
        /// <param name="archive">The active archive to generate name for.</param>
        /// <param name="time">The date that will be in the name, format: yyMMdd</param>
        /// <returns>"[machinename]_[archiveName]_[date(yyMMdd)][_iterationNbr].7z"</returns>
        /// <exception cref="Exception">Throws exception if the are to many archives on the same name format</exception>
        string DecideArchiveName(Archive archive, DateTime time);
    }
}