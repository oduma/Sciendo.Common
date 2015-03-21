using System.Linq;
using NUnit.Framework;
using Sciendo.IOC.Configuration;
using Sciendo.IOC.Tests.SampleLib;
using Sciendo.IOC.Tests.Samples;

namespace Sciendo.IOC.Tests
{
    [TestFixture]
    public class ConfiguredContainerTests
    {
        [Test]
        public void InitializeConfiguredContainerWithDefaultParameters()
        {
            var configuredContainer = IOC.Container.GetInstance().UsingConfiguration();
            Assert.AreEqual(2,configuredContainer.LoadedAssemblies.Keys.Count);
            Assert.AreEqual(1,configuredContainer.LoadedAssemblies["lib"].Count());
            Assert.AreEqual(3, configuredContainer.LoadedAssemblies["all"].Count());
            Assert.Contains("Sciendo.IOC.Tests.SampleLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", configuredContainer.LoadedAssemblies["lib"].Select(a => a.FullName).ToList());
        }
        [Test]
        public void InitializeConfiguredContainerWithGivenParameters()
        {
            var configuredContainer = IOC.Container.GetInstance().UsingConfiguration("myioc",',');
            Assert.AreEqual(2, configuredContainer.LoadedAssemblies.Keys.Count);
            Assert.AreEqual(1, configuredContainer.LoadedAssemblies["lib"].Count());
            Assert.AreEqual(3, configuredContainer.LoadedAssemblies["all"].Count());
            Assert.Contains("Sciendo.IOC.Tests.SampleLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", configuredContainer.LoadedAssemblies["lib"].Select(a => a.FullName).ToList());
            
        }
        [Test]
        public void InitializeConfiguredContainerConfigSectionNotFound()
        {
            var configuredContainer = IOC.Container.GetInstance().UsingConfiguration("noioc", ',');
            Assert.AreEqual(0, configuredContainer.LoadedAssemblies.Keys.Count);
            
        }

        [Test]
        public void AddAllFromFilteredAssemblies()
        {
            var configuredContainer = IOC.Container.GetInstance().UsingConfiguration().AddAllFromFilteredAssemblies<ExtraSampleBase>(LifeStyle.Transient);
            Assert.AreEqual(1,IOC.Container.GetInstance().RegisteredTypes.Count(r => r.Name == "lib"));
            Assert.AreEqual(2, IOC.Container.GetInstance().RegisteredTypes.Count(r => r.Name == "all"));

        }
        [Test]
        public void AddFirstFromFilteredAssemblies()
        {
            var configuredContainer = IOC.Container.GetInstance().UsingConfiguration().AddFirstFromFilteredAssemblies<ISample>(LifeStyle.Transient, "abc", "test property 1", 23);
            Assert.AreEqual(0, IOC.Container.GetInstance().RegisteredTypes.Count(r => r.Name == "libabc"));
            Assert.AreEqual(1, IOC.Container.GetInstance().RegisteredTypes.Count(r => r.Name == "allabc"));

        }

    }
}
