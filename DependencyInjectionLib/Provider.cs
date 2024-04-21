using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionLib
{
    public class Provider
    {
        private Configuration _configuration;

        public Provider(Configuration configuration)
        {
            _configuration = configuration;
        }

        public object Resolve<TType>() where TType : class
        {
            Type type = typeof(TType);
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                Type genericType = type.GetGenericArguments()[0];
                if (_configuration.HasType(genericType))
                {
                    List<object> list = new List<object>();
                    foreach (var implementation in _configuration.GetAllImplementations(genericType))
                    {
                        list.Add(Resolve(implementation));
                    }

                    return list.AsEnumerable();
                }
            }

            if (_configuration.HasType(type))
            {
                Implementation implementation = _configuration.GetFirstImplementation(type);
                return Resolve(implementation) as TType;
            }

            if (type.IsGenericType)
            {
                Type genericTypeDefinition = type.GetGenericTypeDefinition();
                Type genericArgument = type.GetGenericArguments()[0];
                if (_configuration.HasType(genericTypeDefinition) && _configuration.HasType(genericArgument))
                {
                    return ResolveOpenGeneric(_configuration.GetFirstImplementation(genericTypeDefinition).Type,
                        genericArgument) as TType;
                }
            }

            throw new  InvalidOperationException("Unsupported type: " + type.FullName);
        }

        private object CreateObject(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors();

            foreach (var constructorInfo in constructors)
            {
                ParameterInfo[] parameters = constructorInfo.GetParameters();
                if (parameters.All(param => _configuration.HasType(param.ParameterType)))
                {
                    object value = constructorInfo.Invoke(parameters.Select(param =>
                        Resolve(_configuration.GetFirstImplementation(param.ParameterType))).ToArray());
                    return value;
                }
            }

            throw new InvalidOperationException("No suitable constructor for type: " + type.FullName);
        }

        private object ResolveOpenGeneric(Type baseType, Type genericArgumentType)
        {
            return CreateObject(baseType.MakeGenericType(genericArgumentType));
        }

        private object Resolve(Implementation implementation)
        {
            Type type = implementation.Type;
            if (implementation.IsSingleton && implementation.Value != null)
            {
                return implementation.Value;
            }

            object value = CreateObject(type);
            if (implementation.IsSingleton)
            {
                implementation.Value = value;
            }

            return value;
        }
    }
}
