using System.Diagnostics;

namespace IisLogArchiver.Interfaces
{
    public interface IProcessStarter
    {
        ICustomProcess Start(ICustomProcessStartInfo processInfo, ProcessPriorityClass settingsCompressionPriority);
    }
}