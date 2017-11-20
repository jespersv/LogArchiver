using System.Diagnostics;

namespace IisLogArchiver.Interfaces
{
    public class CustomProcess : ICustomProcess
    {
        private readonly Process _process;
        public CustomProcess(Process process)
        {
            _process = process;
        }

        public void WaitForExit()
        {
            _process.WaitForExit();
        }
    }
}