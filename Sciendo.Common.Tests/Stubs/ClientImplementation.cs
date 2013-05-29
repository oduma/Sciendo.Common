using System;
using System.Collections.Generic;
using System.ServiceModel.Description;
using Sciendo.Common.WCF.Clients;

namespace Sciendo.Common.Tests.Stubs
{
    internal class ClientImplementation:BaseClient<ITest1>,ITest1Proxy
    {
        public void Opertaion1()
        {
            throw new NotImplementedException();
        }


        protected override List<IEndpointBehavior> GetEndpointBehaviors()
        {
            return new List<IEndpointBehavior>();
        }
    }
}
