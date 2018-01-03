using System.Collections.Generic;

namespace Sciendo.Common.IO
{
    public interface IFileReaders<out T>
    {
        IEnumerable<T> Read(IEnumerable<string> paths);
    }
}
