using IisLogArchiver.Interfaces;
using System;
using System.Diagnostics;
using System.IO;

namespace IisLogArchiver.Compression
{
    public class CompressorProcessFactory : ICompressProcessFactory
    {
        private readonly string _sevenzipPath;

        public CompressorProcessFactory()
        {
            //todo inject path
            _sevenzipPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"C:\Program Files\7-Zip\7z.exe");
            //_sevenzipPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7za.exe");
        }

        public ICustomProcessStartInfo CreateCompressProcessInfo(string compressTo, string compressFrom)
        {
            return new CustomProcessStartInfo(new ProcessStartInfo
            {
                FileName = _sevenzipPath,
                Arguments = $"a -t7z \"{compressTo}\" \"{compressFrom}\" -mx=9",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false
                //,WorkingDirectory = compressTo
            });
        }

        public ICustomProcessStartInfo CreateTestArchiveProcess(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}