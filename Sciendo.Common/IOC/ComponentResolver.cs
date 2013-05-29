using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Sciendo.Common.Logging;

namespace Sciendo.Common.IOC
{
    public class ComponentResolver
    {
        protected IWindsorContainer Container;

        public void RegisterAll(IWindsorInstaller installer)
        {
            LoggingManager.Debug("Registering all with:" + installer.GetType().ToString());
            using (LoggingManager.LogSciendoPerformance())
            {
                Container = (new WindsorContainer())
                    .Install(installer);
            }
        }

        public T Resolve<T>(string name)
        {
            try
            {
                LoggingManager.Debug("Resolving for " + name);
                using (LoggingManager.LogSciendoPerformance())
                {
                    return Container.Resolve<T>(name);
                }
            }
            catch
            {
                throw new ComponentNotRegieteredException(typeof(T).FullName,
                                                          "A component with the name: " + name + " not registered");
            }
        }


        public T[] ResolveAll<T>()
        {
            try
            {
                LoggingManager.Debug("Resolving all for " + typeof(T).ToString());
                using (LoggingManager.LogSciendoPerformance())
                {

                    return Container.ResolveAll<T>();
                }
            }
            catch
            {
                throw new ComponentNotRegieteredException(typeof(T).FullName, "No components of this type registered.");
            }
        }


    }
}
