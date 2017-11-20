using IisLogArchiver.Interfaces;
using log4net;
using System.IO;

namespace IisLogArchiver.Common
{
    public class FileProvider: IFileProvider
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(FileProvider));
        public bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }

        public void DeleteFile(string file, bool deleteFile)
        {
            if (!deleteFile || !Exists(file))
                return;

            _log.Debug($"Deleting file: {file}");
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        public bool IsFileLocked(string file)
        {
            FileStream stream = null;

            try
            {
                stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None);
                //stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                _log.Error($"File is locked or process is being denied access.");
                return true;
            }
            finally
            {
                stream?.Close();
            }

            //file is not locked
            return false;
        }
    }
}
