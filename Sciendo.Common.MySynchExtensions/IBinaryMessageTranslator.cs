using System.Collections.Generic;

namespace Sciendo.Common.MySynchExtensions
{
    public interface IBinaryMessageTranslator
    {
        byte[] Translate(byte[] inBytes, Dictionary<object, object> findReplacePairs);
    }
}
