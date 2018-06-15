namespace Sciendo.Common.IO
{
    public class FsStorage : IStorage
    {
        public FsStorage()
        {
            Directory=new FSDirectory();
            File= new FSFile();
        }

        public IDirectory Directory { get; private set; }
        public IFile File { get; private set; }
    }
}