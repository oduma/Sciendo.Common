namespace MySynch.Q.Common
{
    public interface IDirectoryMonitor
    {
        event FileSystemEvent Change;
        event FileSystemEvent Delete;
        event FileRenamedEvent Rename;
        void Start();
        void Stop();
    }
}