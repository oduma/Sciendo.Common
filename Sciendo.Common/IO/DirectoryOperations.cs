using System.IO;

namespace Sciendo.Common.IO
{
    public class DirectoryOperations: IDirectoryOperations
    {
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public string SafeCreate(string path)
        {
            if (File.Exists(path))
                return string.Empty;
            if (Directory.Exists(path))
                return path;
            return Directory.CreateDirectory(path).FullName;
        }
    }
}
