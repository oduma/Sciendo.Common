using System;

namespace Sciendo.Common.WCF.Clients
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ApplyCustomClientBehavior : Attribute
    {
        public string BehaviorType { get; set; }
    }
}
