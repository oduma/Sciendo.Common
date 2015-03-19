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
    public class ResolverTests
    {
        Container _container;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _container = Container.GetInstance();
            _container.Add(
                new RegisteredType().For<Sample>().BasedOn<ISample>().With(LifeStyle.Transient),
                new RegisteredType().For<Sample2>().BasedOn<ISample>().With(LifeStyle.Transient).IdentifiedBy("mysecondsample"),
                new RegisteredType().For<ScndSample>().BasedOn<IScndSample>().With(LifeStyle.Singleton),
                new RegisteredType().For<ScndSample2>().BasedOn<IScndSample>().With(LifeStyle.Singleton).IdentifiedBy("myscndsample"),
                new RegisteredType().For<ScndSample>().BasedOn<ISample>().With(LifeStyle.Transient).IdentifiedBy("wrong"));
            AssemblyScanner assemblyScanner = new AssemblyScanner();
            List<Assembly> assemblies = new List<Assembly>();
            foreach (var fileName in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, @"Sciendo.IOC.Tests*.dll"))
            {
                assemblies.Add(Assembly.LoadFrom(fileName));
            }
            _container.Add(assemblyScanner.From(assemblies.ToArray()).BasedOn<ExtraSampleBase>().With(LifeStyle.Transient).ToArray());

        }

        [Test]
        public void ResolveOneWithoutNameTransientOK()
        {
            var result = _container.Resolve<ISample>();
            Assert.IsNotNull(result);
            Assert.True(result is Sample);
            result.Property1 = "property1";
            result = _container.Resolve<ISample>();
            Assert.IsNullOrEmpty(result.Property1);
        }

        [Test]
        public void ResolveOneWithNameTransientOK()
        {
            var result = _container.Resolve<ISample>("mysecondsample");
            Assert.IsNotNull(result);
            Assert.True(result is Sample2);
            result.Property1 = "property1";
            result.Property2 = 22;
            Assert.AreEqual("property1-::-22", result.MixProperties("::"));
            result = _container.Resolve<ISample>("mysecondsample");
            Assert.IsNullOrEmpty(result.Property1);
        }

        [Test]
        public void ResolveOneWithoutNameSingletonOK()
        {
            var result = _container.Resolve<IScndSample>();
            Assert.IsNotNull(result);
            Assert.True(result is ScndSample);
            result.Prop = "property1";
            result = _container.Resolve<IScndSample>();
            Assert.IsNotNullOrEmpty(result.Prop);
            Assert.AreEqual("property1", result.Prop);
        }

        [Test]
        public void ResolveOneWithNameSingletonOK()
        {
            var result = _container.Resolve<IScndSample>("myscndsample");
            Assert.IsNotNull(result);
            Assert.True(result is ScndSample2);
            result.Prop = "property1";
            result = _container.Resolve<IScndSample>("myscndsample");
            Assert.IsNotNullOrEmpty(result.Prop);
            Assert.AreEqual("property1", result.Prop);
        }

        [Test]
        public void ResolveManySingletonOK()
        {
            var result = _container.ResolveAll<IScndSample>();
            Assert.IsNotNull(result);
            Assert.True(result.Any(t=>t is IScndSample));
            Assert.AreEqual(2, result.Count());
            result[0].Prop = "property1";
            result = _container.ResolveAll<IScndSample>();
            Assert.True(result.Any(t=>!string.IsNullOrEmpty(t.Prop)));
            Assert.AreEqual("property1", result[0].Prop);
        }

        [Test]
        public void ResolveManyTransientFromDifferentAssembliesOK()
        {
            var result = _container.ResolveAll<ExtraSampleBase>();
            Assert.IsNotNull(result);
            Assert.True(result.Any(t => t is ExtraSampleBase));
            Assert.AreEqual(4, result.Count());
            result[0].Property = "property1";
            result[1].Property = "property2";
            Assert.AreEqual("property1", result[0].Property);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(NotImplementedException), ExpectedMessage = "Cannot resolve Sciendo.IOC.Tests.Samples.ISample")]
        public void ResolveAWrongRegistration()
        {
            var result = _container.Resolve<ISample>("wrong");

        }

        [Test]
        [ExpectedException(ExpectedException = typeof(NotImplementedException), ExpectedMessage = "Cannot resolve Sciendo.IOC.Tests.Samples.INoSample")]
        public void ResolveANonExistentRegistration()
        {
            var result = _container.Resolve<INoSample>();

        }
        
    }
}
