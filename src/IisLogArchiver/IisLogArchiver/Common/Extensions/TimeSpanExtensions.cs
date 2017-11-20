using System;

namespace IisLogArchiver.Common.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string FmtTimeHMS(this TimeSpan timespan)
        {
            var days = timespan.Days;
            var hours = timespan.Hours;
            var minutes = timespan.Minutes;
            var seconds = timespan.Seconds;
            var ms = timespan.Milliseconds;

            if (days > 0)
                return $"{days}d {hours}h {minutes}m {seconds}s";
            if (hours > 0)
                return $"{hours}h {minutes}m {seconds}s";
            if (minutes > 0)
                return $@"{minutes}m {seconds}s";
            if (seconds > 0)
                return $@"{seconds}s {ms}ms";

            return $@"{ms}ms";
        }
    }
}
