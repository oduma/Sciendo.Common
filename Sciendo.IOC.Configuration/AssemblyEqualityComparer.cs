using System.Collections.Generic;
using System.Reflection;

namespace Sciendo.IOC.Configuration
{
    public class AssemblyEqualityComparer : IEqualityComparer<Assembly>
    {
        public bool Equals(Assembly x, Assembly y)
        {
            if (x == null)
                return false;
            if (y == null)
                return false;
            if (x.FullName != y.FullName)
                return false;
            return true;
        }

        public int GetHashCode(Assembly obj)
        {
            return obj.FullName.GetHashCode();
        }
    }
}
