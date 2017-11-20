using System.Diagnostics;

namespace IisLogArchiver.Common.Extensions
{
    public static class StopwatchExtensions
    {
        public static string ElapsedFmtTimeHMS(this Stopwatch stopwatch)
        {
            return stopwatch.Elapsed.FmtTimeHMS();
        }
    }
}