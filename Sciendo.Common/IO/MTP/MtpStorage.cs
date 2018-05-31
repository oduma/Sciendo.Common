namespace Sciendo.Common.IO.MTP
{
    internal class MtpStorage : IStorage
    {
        public MtpStorage(IDirectory directory, IFile file)
        {
            Directory = directory;
            File = file;
        }

        public IDirectory Directory { get; }
        public IFile File { get; }
    }
}