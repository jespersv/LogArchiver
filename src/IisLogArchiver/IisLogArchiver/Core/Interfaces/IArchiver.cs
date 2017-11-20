using IisLogArchiver.Core;
using System.Collections.Generic;

namespace IisLogArchiver.Interfaces
{
    public interface IArchiver
    {
        /// <summary>
        ///     Processes the logs from the settings to archives. All log files older than today gets rolled into the archive file.
        /// </summary>
        void ProcessLogsToArchives(IEnumerable<Archive> archives);
    }
}