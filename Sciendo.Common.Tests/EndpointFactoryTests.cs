using System.Collections.Generic;
using System.ServiceModel.Description;
using NUnit.Framework;
using Sciendo.Common.Tests.Stubs;
using Sciendo.Common.WCF;
using Sciendo.Common.WCF.Clients;

namespace Sciendo.Common.Tests
{
    [TestFixture]
    public class EndpointFactoryTests
    {
        [Test]
        public void GetChannel_NoChannels_Cached()
        {
            ChannelFactoryPool channelFactoryPool = ChannelFactoryPool.Instance;
            var channelFactory = channelFactoryPool.GetChannelFactory<ITest1>("http://localhost:8000/test1",new List<IEndpointBehavior>{new SciendoAuditBehavior()});
            Assert.IsNotNull(channelFactory);
            Assert.AreEqual("http://localhost:8000/test1",channelFactory.Endpoint.Address.Uri.ToString());
            Assert.IsInstanceOf(typeof(SciendoAuditBehavior),channelFactory.Endpoint.Behaviors[2]);

        }

    }
}
