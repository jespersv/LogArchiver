namespace IisLogArchiver.Interfaces
{
    public interface IDirectoryDeleter
    {
        bool EmptyDirectory(string directoryPath);
    }
}