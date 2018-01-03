﻿using System.IO;
using System.Text;

namespace Sciendo.Common.IO
{
    public class TextFileWriter:IFileWriter
    {
        public void Write(string data, string filePath)
        {
            File.WriteAllText(filePath, data);
        }
    }
}
