namespace Sciendo.Common.IO
{
    public interface IDirectoryOperations
    {
        bool DirectoryExists(string path);

        string SafeCreate(string path);
    }
}
