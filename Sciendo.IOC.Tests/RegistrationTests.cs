using NUnit.Framework;
using Sciendo.IOC.Tests.SampleLib;
using Sciendo.IOC.Tests.Samples;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.IOC.Tests
{
    [TestFixture]
    public class RegistrationTests
    {
        Container _container;

        [SetUp]
        public void SetUp()
        {
            _container = Container.GetInstance();
            var ms = _container.RegisteredTypes.FirstOrDefault(t => t.Name == "mysample");
            if (ms!=null)
                _container.RegisteredTypes.Remove(ms);
            var ss = _container.RegisteredTypes.Where(t=>t.Name=="Sciendo.IOC.Tests.Samples.ISample").ToList();
            foreach(var s in ss)
                _container.RegisteredTypes.Remove(s);
        }
        [Test]
        public void CreateAContainer()
        {
            Assert.IsNotNull(_container);
            Assert.IsNotNull(_container.RegisteredTypes);
        }

        [Test]
        public void AddAFullRegistrationToContainer()
        {
            _container.Add(new RegisteredType().For<Sample>().BasedOn<ISample>().IdentifiedBy("mysample").With(LifeStyle.Transient));
            var ms = _container.RegisteredTypes.FirstOrDefault(k => k.Name == "mysample");
            Assert.IsNotNull(ms);
            Assert.AreEqual(typeof(Sample), ms.Implementation);
            Assert.AreEqual(typeof(ISample), ms.Service);
            Assert.AreEqual("mysample", ms.Name);
            Assert.AreEqual(LifeStyle.Transient, ms.LifeStyle);
        }

        [Test]
        public void AddAFullRegistrationWithParametrisedConstructorToContainer()
        {
            _container.Add(new RegisteredType().For<Sample>().BasedOn<ISample>().IdentifiedBy("mysample1").With(LifeStyle.Transient).WithConstructorParameters("test property 1",23));
            var ms = _container.RegisteredTypes.FirstOrDefault(k => k.Name == "mysample1");
            Assert.IsNotNull(ms);
            Assert.AreEqual("test property 1",((Sample)ms.Instance).Property1);
            Assert.AreEqual(23, ((Sample)ms.Instance).Property2);
            Assert.AreEqual(typeof(Sample), ms.Implementation);
            Assert.AreEqual(typeof(ISample), ms.Service);
            Assert.AreEqual("mysample1", ms.Name);
            Assert.AreEqual(LifeStyle.Transient, ms.LifeStyle);
        }
        [Test]
        public void AddANamelessRegistrationToContainer()
        {
            _container.Add(new RegisteredType().For<Sample>().BasedOn<ISample>().With(LifeStyle.Transient));
            var ms = _container.RegisteredTypes.FirstOrDefault(k => k.Name == "Sciendo.IOC.Tests.Samples.ISample");
            Assert.IsNotNull(ms);
            Assert.AreEqual(typeof(Sample), ms.Implementation);
            Assert.AreEqual(typeof(ISample), ms.Service);
            Assert.AreEqual("Sciendo.IOC.Tests.Samples.ISample", ms.Name);
            Assert.AreEqual(LifeStyle.Transient, ms.LifeStyle);
        }

        [Test]
        [Ignore]
        public void AddAllRegistrationsFromThisAssembliesToContainer()
        {
            AssemblyScanner assemblyScanner= new AssemblyScanner();
            _container.Add(assemblyScanner.From(Assembly.GetExecutingAssembly()).BasedOn<ISample>().With(LifeStyle.Transient).ToArray());
            Assert.IsNotNull(_container.RegisteredTypes);
            Assert.AreEqual(2, _container.RegisteredTypes.Count);
            Assert.AreEqual(2, _container.RegisteredTypes.Count(t => t.Service ==typeof(ISample)));
            Assert.AreEqual(1, _container.RegisteredTypes.Count(t => t.Implementation ==typeof(Sample)));
            Assert.AreEqual(1, _container.RegisteredTypes.Count(t => t.Implementation ==typeof(Sample2)));
        }

        [Test]
        [Ignore]
        public void AddAllRegistrationsFromAnyAssembliesToContainer()
        {
            AssemblyScanner assemblyScanner = new AssemblyScanner();
            List<Assembly> assemblies = new List<Assembly>();
            foreach(var fileName in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, @"Sciendo.IOC.Tests*.dll"))
            {
                assemblies.Add(Assembly.LoadFrom(fileName));
            }
            _container.Add(assemblyScanner.From(assemblies.ToArray()).BasedOn<ExtraSampleBase>().With(LifeStyle.Transient).ToArray());
            Assert.IsNotNull(_container.RegisteredTypes);
            Assert.AreEqual(2, _container.RegisteredTypes.Count);
            Assert.AreEqual(2, _container.RegisteredTypes.Count(t => t.Service == typeof(ExtraSampleBase)));
            Assert.AreEqual(1, _container.RegisteredTypes.Count(t => t.Implementation == typeof(Sciendo.IOC.Tests.Samples.Class1)));
            Assert.AreEqual(1, _container.RegisteredTypes.Count(t => t.Implementation == typeof(Sciendo.IOC.Tests.SampleLib.Class1)));
        }
        [Test]
        public void AddAllRegistrationsFromAnyAssembliesToContainerWithASingleName()
        {
            AssemblyScanner assemblyScanner = new AssemblyScanner();
            List<Assembly> assemblies = new List<Assembly>();
            foreach (var fileName in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, @"Sciendo.IOC.Tests.dll"))
            {
                assemblies.Add(Assembly.LoadFrom(fileName));
            }
            _container.Add(assemblyScanner.From(assemblies.ToArray()).BasedOn<ExtraSampleBase>().IdentifiedBy("myownsamples").With(LifeStyle.Transient).ToArray());
            List<Assembly> assemblies2 = new List<Assembly>();
            foreach (var fileName in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, @"Sciendo.IOC.Tests.SampleLib.dll"))
            {
                assemblies2.Add(Assembly.LoadFrom(fileName));
            }
            _container.Add(assemblyScanner.From(assemblies2.ToArray()).BasedOn<ExtraSampleBase>().IdentifiedBy("extrasamples").With(LifeStyle.Transient).ToArray());
            Assert.IsNotNull(_container.RegisteredTypes);
            Assert.AreEqual(2, _container.RegisteredTypes.Count);
            Assert.AreEqual(1, _container.RegisteredTypes.Count(t => t.Name == "myownsamples"));
            Assert.AreEqual(2, _container.RegisteredTypes.Count(t => t.Service == typeof(ExtraSampleBase)));
            Assert.AreEqual(1, _container.RegisteredTypes.Count(t => t.Implementation == typeof(Sciendo.IOC.Tests.Samples.Class1)));
            Assert.AreEqual(1, _container.RegisteredTypes.Count(t => t.Implementation == typeof(Sciendo.IOC.Tests.SampleLib.Class1)));
            Assert.AreEqual(1, _container.RegisteredTypes.Count(t => t.Name == "extrasamples"));

        }
    }
}
