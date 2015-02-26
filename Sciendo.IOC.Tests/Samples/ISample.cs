using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.IOC.Tests.Samples
{
    public interface ISample
    {
        string Property1 { get; set; }

        int Property2 { get; set; }

        string MixProperties(string connector);
    }
}
