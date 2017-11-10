namespace Sciendo.Common.IO
{
    public interface IFileOperations
    {
        bool FileExists(string path);

        string ReadAllText(string path);

        void WriteAllText(string path, string text);
    }
}
