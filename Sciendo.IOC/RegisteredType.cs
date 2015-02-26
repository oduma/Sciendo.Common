using System;

namespace Sciendo.IOC
{
    public enum LifeStyle
    {
        Transient,
        Singleton
    }

    public class RegisteredType
    {
        internal Type Service { get; set; }

        internal Type Implementation { get; set; }

        private object _instance;

        private object[] _params;

        public RegisteredType WithConstructorParameters(params object[] parameters)
        {
            _params = parameters;
            return this;
        }

        internal object Instance { 
            get 
        {

            if ((_instance == null)
                || (_instance != null && LifeStyle == LifeStyle.Transient))
            {
                if (_params != null && _params.Length > 0)
                {
                    _instance = Activator.CreateInstance(Implementation, _params);
                }
                else
                {
                    _instance = Activator.CreateInstance(Implementation);
                }
            }
            return _instance;

        }
        }

        internal string Name { get; set; }

        internal LifeStyle LifeStyle { get; set; }

        public RegisteredType For<T>()
        {
            Implementation = typeof(T);
            return this;
        }

        public RegisteredType BasedOn<T>()
        {
            Service = typeof(T);
            if (string.IsNullOrEmpty(Name))
                Name = Service.FullName;
            return this;
        }

        public RegisteredType IdentifiedBy(string name)
        {
            Name = name;
            return this;
        }

        public RegisteredType With(LifeStyle lifestyle)
        {
            LifeStyle = lifestyle;
            return this;
        }

        internal bool Validate()
        {
            return (Service != null && Implementation != null && Service.IsAssignableFrom(Implementation));
        }

        internal bool IsSuitable<T>(string key)
        {
            return (Name==key && Service==typeof(T) && typeof(T).IsAssignableFrom(Implementation));
        }
        internal bool IsSuitable<T>()
        {
            return (Service == typeof(T) && typeof(T).IsAssignableFrom(Implementation));
        }
    }
}
