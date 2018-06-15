using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.Common.IO

{
    public class FSFile : IFile
    {
        public void WriteAllText(string fileName, string content)
        {
            File.WriteAllText(fileName, content);
        }

        public void Create(string path,byte[] body)
        {

            using (var fs = File.Create(path, body.Length))
            {
                fs.Write(body, 0, body.Length);
            }
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public void Delete(string path)
        {
            File.Delete(path);
        }

    }
}
