using System.IO;

namespace Sciendo.Common.IO
{
    public class ContentMover:ContentWriterBase
    {
        public ContentMover(IDirectory directory) : base(directory)
        {
        }

        public override void Do(string fromPath, string toPath)
        {
            DoOperation(fromPath, toPath, File.Move);
        }
    }
}
