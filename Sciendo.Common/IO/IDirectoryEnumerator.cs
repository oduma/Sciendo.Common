using System.Collections.Generic;

namespace Sciendo.Common.IO
{
    public interface IDirectoryEnumerator
    {

        IEnumerable<string> GetTopLevel(string path);

    }
}
