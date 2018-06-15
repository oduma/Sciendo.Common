using System.IO;

namespace Sciendo.Common.IO
{
    public class ContentCopier:ContentWriterBase
    {
        public ContentCopier(IDirectory directory) : base(directory)
        {
        }

        public override void Do(string fromPath, string toPath)
        {
            DoOperation(fromPath,toPath,File.Copy);
        }
    }
}
