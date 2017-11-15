using System.Collections.Generic;

namespace Sciendo.Common.MySynchExtensions
{
    public interface IMessageTranslator
    {
        byte[] Translate(byte[] inBytes, Dictionary<string, string> findReplacePairs);
    }
}
