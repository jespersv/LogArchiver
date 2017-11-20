using IisLogArchiver.Interfaces;
using System.Threading;

namespace IisLogArchiver.Common
{
    public class ThreadProvider : IThreadProvider
    {
        public void Sleep(int timeMs)
        {
            Thread.Sleep(timeMs);
        }
    }
}