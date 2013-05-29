using NUnit.Framework;
using Sciendo.Common.Tests.Stubs;
using Sciendo.Common.WCF.Clients;

namespace Sciendo.Common.Tests
{
    [TestFixture]
    public class BaseClientTests
    {
        [Test]
        public void CreateABaseClient_Ok()
        {
            ITest1Proxy baseClient= new ClientImplementation();
            Assert.IsNotNull(baseClient);
        }

        [Test]
        public void InitateEndpointByServerAddress_Ok()
        {
            var baseClient = new ClientImplementation();
            baseClient.InitiateUsingServerAddress("http://myserver/myservice");
            Assert.AreEqual(1,ChannelFactoryPool.Instance.ClientEndpoints.Keys.Count);
            Assert.IsNotNull(ChannelFactoryPool.Instance.ClientEndpoints[@"ITest1http://myserver/myservice"]);
        }
    }
}
