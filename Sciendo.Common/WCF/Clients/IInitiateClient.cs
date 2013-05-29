using System.ServiceModel;

namespace Sciendo.Common.WCF.Clients
{
    public interface IInitiateClient
    {
        void InitiateUsingServerAddress(string serverAddress);
        void InitiateDuplexUsingServerAddress(string serverAddress, InstanceContext callbackInstance);
        void Reset();
    }
}
