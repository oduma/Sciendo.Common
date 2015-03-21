using System.Configuration;

namespace Sciendo.IOC.Configuration
{
    public class IOCConfigurationSection:ConfigurationSection
    {
        [ConfigurationProperty("assemblies")]
        public IOCAssembliesConfigurationCollection Assemblies
        {
            get { return (IOCAssembliesConfigurationCollection)this["assemblies"]; }
            set { this["assemblies"] = value; }
        }

        public override string ToString()
        {
            return string.Format("Assemblies: {0}.", Assemblies.ToString());
        }
    }
}
