using System;
using IisLogArchiver.Interfaces;

namespace IisLogArchiver.Common
{
    public class TimeProvider : ITimeProvider
    {
        public DateTime Now => DateTime.UtcNow;
    }
}