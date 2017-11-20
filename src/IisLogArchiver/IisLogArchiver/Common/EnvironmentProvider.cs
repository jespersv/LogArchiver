using IisLogArchiver.Interfaces;
using System;

namespace IisLogArchiver.Common
{
    public class EnvironmentProvider : IEnvironmentProvider
    {
        public string MachineName => Environment.MachineName;
    }
}