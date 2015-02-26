using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.IOC.Tests.Samples
{
    public class Sample:ISample
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
            return string.Format("{0}{1}{2}", Property1, connector, Property2);
        }

        public Sample()
        {
            
        }

        public Sample(string property1, int property2)
        {
            Property1 = property1;
            Property2 = property2;
        }
    }
}
