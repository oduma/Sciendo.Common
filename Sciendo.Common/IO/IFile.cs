using System;
using System.Collections.Generic;
using System.IO;

namespace Sciendo.Common.IO
{
    public interface IFile
    {
        void WriteAllText(string fileName, string content);
        void Create(string localFileName, byte[] body);
        bool Exists(string path);
        void Delete(string path);
    }
}