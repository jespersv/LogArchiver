namespace IisLogArchiver.Interfaces
{
    public interface ICompressProcessFactory
    {
        ICustomProcessStartInfo CreateCompressProcessInfo(string compressTo, string compressFrom);
        ICustomProcessStartInfo CreateTestArchiveProcess(string filePath);
    }
}