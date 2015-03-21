using System.Configuration;

namespace Sciendo.IOC.Configuration
{
    public class AssemblyConfiguration:ConfigurationElement
    {
        [ConfigurationProperty("key", DefaultValue = "", IsRequired = true, IsKey = true)]
        public string Key
        {
            get { return (string) this["key"]; }
            set { this["key"] = value; }
        }

        [ConfigurationProperty("assemblyFilter", DefaultValue = "", IsRequired = true)]
        public string AssemblyFilter
        {
            get { return (string)this["assemblyFilter"]; }
            set { this["assemblyFilter"] = value; }
        }

        public override string ToString()
        {
            return string.Format("Key: {0} Assemblies Filter: {1}", Key, AssemblyFilter);
        }
    }
}
