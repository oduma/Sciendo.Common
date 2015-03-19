using System.Collections.Generic;
using System.Linq;

namespace Sciendo.IOC
{
    public static class ExtensionMethods
    {
        public static IEnumerable<RegisteredType> With(this IEnumerable<RegisteredType> inTypes, LifeStyle lifeStyle)
        {
            return inTypes.Select(inType => inType.With(lifeStyle));
        }

        public static IEnumerable<RegisteredType> IdentifiedBy(this IEnumerable<RegisteredType> inTypes, string name)
        {
            foreach (var inType in inTypes)
            {
                inType.IdentifiedBy(name);
                yield return inType;
            }
        }
    }
}
