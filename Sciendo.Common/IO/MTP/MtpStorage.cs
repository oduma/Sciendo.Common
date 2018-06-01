namespace Sciendo.Common.IO.MTP
{
    public class MtpStorage : IStorage
    {
        public MtpStorage()
        {
            Directory = new MtpDirectory();
            File = new MtpFile();
        }

        public IDirectory Directory { get; }
        public IFile File { get; }
    }
}