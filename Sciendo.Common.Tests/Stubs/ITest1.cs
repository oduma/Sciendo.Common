using System.ServiceModel;

namespace Sciendo.Common.Tests.Stubs
{
    [ServiceContract]
    internal interface ITest1
    {
        [OperationContract]
        void Opertaion1();
    }
}
