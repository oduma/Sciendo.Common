using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Sciendo.Common.WCF;

namespace Sciendo.Common.WCF
{
    public class SciendoAuditBehavior : BehaviorExtensionElement, IEndpointBehavior
    {
        #region IEndpointBehavior Members

        public void Validate(ServiceEndpoint endpoint)
        {
            return;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            return;
        }

        /// <summary>
        /// Applies the behavior on the server side
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="endpointDispatcher"></param>
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            SciendoAuditMessageInspector inspector = new SciendoAuditMessageInspector();
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
        }

        /// <summary>
        /// Applies the behavior on the client side
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="clientRuntime"></param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            SciendoAuditMessageInspector inspector = new SciendoAuditMessageInspector();
            clientRuntime.MessageInspectors.Add(inspector);
        }

        #endregion

        protected override object CreateBehavior()
        {
            return new SciendoAuditBehavior();
        }

        public override Type BehaviorType
        {
            get { return typeof(SciendoAuditBehavior); }
        }
    }
}