namespace IisLogArchiver.Interfaces
{
    public interface IFileProvider
    {
        bool Exists(string filePath);
        void DeleteFile(string file, bool deleteFile);
        bool IsFileLocked(string file);
    }
}