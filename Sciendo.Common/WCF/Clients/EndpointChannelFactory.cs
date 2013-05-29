using System.ServiceModel;

namespace Sciendo.Common.WCF.Clients
{
    public class EndpointChannelFactory
    {
        public EndpointAddress EndpointAddress { get; set; }

        public ChannelFactory ChannelFactory { get; set; }
    }
}
