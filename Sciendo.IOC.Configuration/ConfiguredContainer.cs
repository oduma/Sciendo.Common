using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using Sciendo.Common.Logging;

namespace Sciendo.IOC.Configuration
{
    public class ConfiguredContainer
    {
        internal IOCConfigurationSection CurrentConfiguration;
        internal Dictionary<string, IEnumerable<Assembly>> LoadedAssemblies;
        private readonly AssemblyScanner _scanner = new AssemblyScanner();
        private readonly Container _container;

        protected ConfiguredContainer()
        {
            CurrentConfiguration = null;
            LoadedAssemblies = new Dictionary<string, IEnumerable<Assembly>>();   
        }

        internal ConfiguredContainer(Container container, string configurationSectionName, char assemblyFiltersSeparator)
        {
            _container = container;
            CurrentConfiguration = ConfigurationManager.GetSection(configurationSectionName) as IOCConfigurationSection;
            LoadedAssemblies = new Dictionary<string, IEnumerable<Assembly>>();
            if (CurrentConfiguration != null)
            {
                foreach (var assemblyConfiguration in CurrentConfiguration.Assemblies.AsEnumerable())
                {
                    LoadedAssemblies.Add(assemblyConfiguration.Key, GetAssemblies(assemblyConfiguration.AssemblyFilter, assemblyFiltersSeparator));
                }
            }

        }

        private static IEnumerable<Assembly> GetAssemblies(string assemblyFilters, char assemblyFiltersSeparator)
        {
            foreach (var assemblyFilter in assemblyFilters.Split(new[] { assemblyFiltersSeparator }))
            {
                var foundAssemblyForFilter = false;
                foreach (var fileName in GetFiles(assemblyFilter))
                {
                    Assembly assembly = null;
                    try
                    {
                        assembly = Assembly.LoadFrom(fileName);
                        foundAssemblyForFilter = true;
                    }
                    catch (Exception ex)
                    {
                        LoggingManager.LogSciendoSystemError("Assembly Load Error for assembly: " + fileName, ex);
                    }
                    if (assembly != null)
                    {
                        yield return assembly;
                    }
                }
                if(!foundAssemblyForFilter)
                    LoggingManager.Debug("No assembly found for filter " + assemblyFilter +" in folder: " + AppDomain.CurrentDomain.BaseDirectory + " or its bin sub folder.");
            }
        }

        private static string[] GetFiles(string assemblyFilter)
        {
            var filteredFiles= Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, assemblyFilter);
            if (filteredFiles.Length == 0)
            {
                var alternatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
                if(Directory.Exists(alternatePath))
                    filteredFiles = Directory.GetFiles(alternatePath,
                        assemblyFilter);
            }
            return filteredFiles;
        }

        public ConfiguredContainer AddAllFromFilteredAssemblies<T>(LifeStyle lifeStyle, string additionalQualifier = null)
        {
            if (LoadedAssemblies != null && LoadedAssemblies.Any())
            {
                foreach (var key in LoadedAssemblies.Keys)
                {
                    var regTypes =
                        _scanner.From(LoadedAssemblies[key].Distinct(new AssemblyEqualityComparer()).ToArray())
                            .BasedOn<T>()
                            .IdentifiedBy(key + ((additionalQualifier) ?? ""))
                            .With(lifeStyle).ToArray();
                    if (!regTypes.Any())
                    {
                        LoggingManager.Debug(
                            string.Format(
                                "No {0} implementation found in any of the assemblies {1} registered under they key: {2}",
                                typeof(T).FullName, string.Join(", ", LoadedAssemblies[key].Select(a => a.FullName)), key)
                            );
                    }
                    _container.Add(regTypes);
                }
            }
            return this;
        }

        public ConfiguredContainer AddFirstFromFilteredAssemblies<T>(LifeStyle lifeStyle, string additionalQualifier = null, params object[] constructorParams)
        {
            foreach (var key in LoadedAssemblies.Keys)
            {
                var regType =
                    _scanner.From(LoadedAssemblies[key].Distinct(new AssemblyEqualityComparer()).ToArray())
                        .BasedOn<T>()
                        .IdentifiedBy(key + ((additionalQualifier) ?? ""))
                        .With(lifeStyle).FirstOrDefault();
                if (regType == null)
                {
                    LoggingManager.Debug(
                        string.Format(
                            "No {0} implementation found in any of the assemblies {1} registered under they key: {2}",
                            typeof(T).FullName, string.Join(", ", LoadedAssemblies[key].Select(a => a.FullName)), key)
                        );
                }
                else
                {
                    _container.Add(regType.WithConstructorParameters(constructorParams));
                }
            }
            return this;
        }

    }
}
