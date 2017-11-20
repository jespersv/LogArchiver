using IisLogArchiver.Interfaces;
using System.Diagnostics;

namespace IisLogArchiver.Compression
{
    public class CustomProcessStartInfo : ICustomProcessStartInfo
    {
        public ProcessStartInfo ProcessStartInfo { get; }

        public CustomProcessStartInfo(ProcessStartInfo processStartInfo)
        {
            ProcessStartInfo = processStartInfo;
        }
    }
}