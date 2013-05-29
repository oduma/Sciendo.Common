﻿using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceProcess;
using Sciendo.Common.Logging;

namespace Sciendo.Common.WCF
{
    public abstract class WcfHostServiceBase:ServiceBase
    {
        protected List<ServiceHost> _serviceHosts = new List<ServiceHost>();


        protected void CloseAllServiceHosts()
        {
            if (_serviceHosts != null)
            {
                _serviceHosts.ForEach(CloseServiceHost);
            }

        }
        protected void OpenAllServiceHosts()
        {
            _serviceHosts.ForEach(OpenServiceHost);
        }

        private void OpenServiceHost(ServiceHost serviceHost)
        {
            serviceHost.Open();
            LoggingManager.Debug("Opened Host: " + serviceHost.BaseAddresses[0].ToString());
        }

        private void CloseServiceHost(ServiceHost serviceHost)
        {
            LoggingManager.Debug("Closed Host: " + serviceHost.BaseAddresses[0].ToString());
            serviceHost.Close();
        }

        protected virtual ServiceHost CreateAndConfigureServiceHost<T>(T serviceInstance, Uri baseAddress, bool isDuplex=false)
        {
            var serviceHost = new ServiceHost(serviceInstance, baseAddress);
            var serviceEndPoint = serviceHost.AddServiceEndpoint(typeof(T), ClientServerBindingHelper.GetBinding(isDuplex), string.Empty);

            serviceEndPoint.Behaviors.Add(new SciendoAuditBehavior());

            return serviceHost;
        }


    }
}
