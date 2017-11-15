using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.Common.MySynchExtensions
{
    public interface ITextMessageTranslator
    {
        string Translate(string inString, Dictionary<string, string> findReplacePairs);
    }
}
