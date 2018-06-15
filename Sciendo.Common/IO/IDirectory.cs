using System;
using System.Collections.Generic;
using System.IO;

namespace Sciendo.Common.IO
{
    public interface IDirectory
    {
        bool Exists(string path);
        void CreateDirectory(string path);
        void Delete(string path, bool contentsAlso);
        IEnumerable<string> GetTopLevel(string path);

        IEnumerable<string> GetFiles(string path, SearchOption searchOption, string[] extensions = null);

        event EventHandler<DirectoryReadEventArgs> DirectoryRead;

        event EventHandler<ExtensionsReadEventArgs> ExtensionsRead;

    }
}