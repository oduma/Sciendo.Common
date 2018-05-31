namespace Sciendo.Common.IO
{
    public interface IDirectory
    {
        bool Exists(string path);
        void CreateDirectory(string path);
        void Delete(string path, bool contentsAlso);
    }
}