using System;

namespace IisLogArchiver.Interfaces
{
    public interface ITimeProvider
    {
        DateTime Now { get; }
    }
}