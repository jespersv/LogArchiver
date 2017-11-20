using IisLogArchiver.Interfaces;
using log4net;
using System;
using System.Diagnostics;
using System.IO;

namespace IisLogArchiver.Compression
{
    public class Compressor : ICompressor
    {
        private readonly ICompressProcessFactory _processFactory;
        private readonly IFileDeleter _fileDeleter;
        private readonly IProcessStarter _processStarter;
        private readonly IArchiveSettings _settings;
        private static readonly ILog Log = LogManager.GetLogger(typeof(Compressor));
        private readonly string _sevenzipPath;

        public Compressor(
            ICompressProcessFactory processFactory,
            IFileDeleter fileDeleter,
            IProcessStarter processStarter,
            IArchiveSettings settings
            )
        {
            _processFactory = processFactory;
            _fileDeleter = fileDeleter;
            _processStarter = processStarter;
            _settings = settings;
        }

        public bool Compress(string compressTo, params string[] compressFromFiles)
        {
            if (string.IsNullOrEmpty(compressTo))
                throw new ArgumentException(nameof(compressTo));
            if (compressFromFiles == null || compressFromFiles.Length == 0)
                throw new ArgumentException(nameof(compressFromFiles));

            var failed = false;
            var compressFrom = string.Join("\" \"", compressFromFiles);
            var p = _processFactory.CreateCompressProcessInfo(compressTo, compressFrom);

            var watch = Stopwatch.StartNew();
            Log.Info($"Starting compression");
            try
            {
                var process = _processStarter.Start(p, _settings.CompressionPriority);
                process.WaitForExit();
            }
            catch (Exception e)
            {
                Log.Info($"Exception occurred during compression to archive.");
                Log.Info(e.StackTrace);
                Log.Info($"Deleting incomplete archive");
                _fileDeleter.TryDeleteFile(compressTo, true);
                failed = true;
            }

            //todo logic for verification of archive

            var archiveName = Path.GetFileName(compressTo);
            watch.Stop();
            Log.Info($"Compressing {archiveName} took {watch.Elapsed.Minutes}m {watch.Elapsed.Seconds}s");
            return failed;
        }

        //note, not done and not in use
        public void Extract(string source, string destination)
        {
            // If the directory doesn't exist, create it.
            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);

            // change the path and give yours 
            try
            {
                var pro = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = _sevenzipPath,
                    Arguments = $"x \"{source}\" -o {destination}"
                };
                var x = Process.Start(pro);
                x.WaitForExit();
            }
            catch (Exception Ex)
            {
            }
        }
    }
}