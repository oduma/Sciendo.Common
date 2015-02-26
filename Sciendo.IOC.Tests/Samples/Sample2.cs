using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.IOC.Tests.Samples
{
    public class Sample2:ISample
    {
        public string Property1
        {
            get;
            set;
        }

        public int Property2
        {
            get;
            set;
        }

        public string MixProperties(string connector)
        {
            return string.Format("{0}-{1}-{2}", Property1, connector, Property2);
        }
    }
}
