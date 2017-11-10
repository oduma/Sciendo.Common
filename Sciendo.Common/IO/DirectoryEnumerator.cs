using System;
using System.Collections.Generic;
using System.IO;

namespace Sciendo.Common.IO
{
    public class DirectoryEnumerator:IDirectoryEnumerator
    {
        public IEnumerable<string> GetTopLevel(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (!Directory.Exists(path))
                throw new ArgumentException("Folder does not exist.", nameof(path));
            return Directory.EnumerateDirectories(path, "*.*", SearchOption.TopDirectoryOnly);

        }
    }
}
