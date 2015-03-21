namespace Sciendo.IOC.Configuration
{
    public static class ContainerExtension
    {
        public static ConfiguredContainer UsingConfiguration(this Container container, string configurationSectionName="ioc",
            char assemblyFiltersSeparator = ';')
        {
            return new ConfiguredContainer(container,configurationSectionName, assemblyFiltersSeparator);
        }


    }
}
