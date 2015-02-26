using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sciendo.IOC
{
    public class AssemblyScanner
    {
        private Assembly[] _assemblies;

        public AssemblyScanner From(params Assembly[] inAssemblies)
        {
            _assemblies = inAssemblies;
            return this;
        }

        public IEnumerable<RegisteredType> BasedOn<T>()
        {
            return from assembly 
                       in _assemblies 
                   from type in 
                   assembly.GetTypes().Where(t=>t.IsClass && !t.IsAbstract && typeof(T).IsAssignableFrom(t)) 
                   select new RegisteredType { Implementation = type, Service = typeof(T), Name = typeof(T).FullName };
        }
    }
}
