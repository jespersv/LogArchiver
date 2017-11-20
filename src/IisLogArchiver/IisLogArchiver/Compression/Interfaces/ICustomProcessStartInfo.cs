using System.Diagnostics;

namespace IisLogArchiver.Interfaces
{
    public interface ICustomProcessStartInfo
    {
        ProcessStartInfo ProcessStartInfo { get; }
    }
}