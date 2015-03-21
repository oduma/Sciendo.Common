using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Sciendo.IOC.Configuration
{
    [ConfigurationCollection(typeof(AssemblyConfiguration))]
    public class IOCAssembliesConfigurationCollection:ConfigurationElementCollection
    {
        public AssemblyConfiguration this[int index ]
        {
            get { return (AssemblyConfiguration) BaseGet(index); }
            set
            {
                if(BaseGet(index)!=null)
                    BaseRemoveAt(index);
                BaseAdd(index,value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new AssemblyConfiguration();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AssemblyConfiguration) element).Key;
        }

        internal IEnumerable<AssemblyConfiguration> AsEnumerable()
        {
            for (int i = 0; i < Count; i++)
                yield return this[i];
        }
        public override string ToString()
        {
            return string.Join("\r\n", AsEnumerable().Select(a => a.ToString()));
        }
    }
}
