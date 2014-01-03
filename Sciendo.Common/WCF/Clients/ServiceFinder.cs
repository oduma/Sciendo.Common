using System;
using System.Linq;
using System.ServiceModel.Discovery;
using Sciendo.Common.Logging;

namespace Sciendo.Common.WCF.Clients
{
    public static class ServiceFinder
    {
        public static string[] FindServices(Type serviceType)
        {
            LoggingManager.Debug("Looking for service of type " + serviceType.FullName);
            try
            {
                DiscoveryClient discoveryClient =
                    new DiscoveryClient(new UdpDiscoveryEndpoint());

                var discoveredServices =
                    discoveryClient.Find(new FindCriteria(serviceType));

                discoveryClient.Close();

                var serviceEndpointAdresses = discoveredServices.Endpoints.Select(e=>e.Address.ToString());
                if (!serviceEndpointAdresses.Any() )
                    LoggingManager.Debug("No service of type " + serviceType.FullName);
                else
                    LoggingManager.Debug("Found services.");
                return serviceEndpointAdresses.ToArray();
            }
            catch (Exception ex)
            {
                LoggingManager.LogSciendoSystemError(ex);
                throw;
            }

        }
    }
}
