using System;

namespace Sciendo.Common.IOC
{
    public class ComponentNotRegieteredException:Exception
    {
        public string ComponentName { get; set; }
        public ComponentNotRegieteredException(string componentName, string message):base(message)
        {
            ComponentName = componentName;
        }
    }
}
