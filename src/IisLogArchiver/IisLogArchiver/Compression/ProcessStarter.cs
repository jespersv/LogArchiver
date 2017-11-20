using IisLogArchiver.Interfaces;
using System.Diagnostics;

namespace IisLogArchiver.Compression
{
    public class ProcessStarter : IProcessStarter
    {
        private Process _process;
        public ICustomProcess Start(ICustomProcessStartInfo processInfo, ProcessPriorityClass priority)
        {
            _process = Process.Start(processInfo.ProcessStartInfo);
            if (_process != null)
                _process.PriorityClass = priority;
            return new CustomProcess(_process);
        }
    }
}