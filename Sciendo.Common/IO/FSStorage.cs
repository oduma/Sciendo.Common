namespace Sciendo.Common.IO
{
    internal class FSStorage : IStorage
    {
        public FSStorage()
        {
            Directory=new FSDirectory();
            File= new FSFile();
        }
        public IDirectory Directory { get; private set; }
        public IFile File { get; private set; }
    }
}