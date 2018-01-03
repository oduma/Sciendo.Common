using System.Collections.Generic;

namespace Sciendo.Common.IO
{
    public interface IFilesReader<out T>
    {
        IEnumerable<T> Read(IEnumerable<string> paths);
    }
}
