namespace IisLogArchiver.Interfaces
{
    public interface ICompressor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="compressTo"></param>
        /// <param name="compressFrom"></param>
        /// <returns>If completed without error.</returns>
        bool Compress(string compressTo, params string[] compressFrom);
    }
}