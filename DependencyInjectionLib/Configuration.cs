namespace DependencyInjectionLib
{
    public class Configuration
    {
        private readonly Dictionary<Type, List<Implementation>> _configuration = new();

        public void Register<TType, TImplementation>(bool isSingleton = true) 
        {
            if (!_configuration.ContainsKey(typeof(TType)))
            {
                _configuration[typeof(TType)] = new List<Implementation>();
            }
            _configuration[typeof(TType)].Add(new Implementation(typeof(TImplementation), isSingleton));
        }

        public void Register(Type dependency, Type implementation, bool isSingleton = false) 
        {
            if (!_configuration.ContainsKey(dependency))
            {
                _configuration[dependency] = new List<Implementation>();
            }
            _configuration[dependency].Add(new Implementation(implementation, isSingleton));
        }

        public bool HasType(Type type)
        {
            return _configuration.ContainsKey(type);
        }

        public Implementation GetFirstImplementation(Type key)
        {
            return _configuration[key].First();
        }

        public List<Implementation> GetAllImplementations(Type key)
        {
            return _configuration[key];
        }

    }
}