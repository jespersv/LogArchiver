using IisLogArchiver.Core;
using System.Diagnostics;

namespace IisLogArchiver.Interfaces
{
    public interface IArchiveSettings
    {
        ArchiveSettingsRootObject SettingsRootObject { get; }
        int ArgumentLengthBeforePerCompress { get; }
        ProcessPriorityClass CompressionPriority { get; }
    }
}