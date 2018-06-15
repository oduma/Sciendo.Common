namespace Sciendo.Common.IO
{
    public interface IStorage
    {
        IDirectory Directory { get; }

        IFile File { get; }
       
    }
}
