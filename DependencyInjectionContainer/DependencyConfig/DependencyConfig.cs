using System;
using System.Collections.Generic;
using System.Linq;
using DependencyInjectionContainer.ExtensionMethodForTypeClass;

namespace DependencyInjectionContainer.DependencyConfig
{
    public class DependencyConfig : IDependencyConfig
    {
        private readonly Dictionary<Type, List<ImplementationInfo.ImplementationInfo>> _dependenciesImplementations;

        public DependencyConfig() => _dependenciesImplementations = 
            new Dictionary<Type, List<ImplementationInfo.ImplementationInfo>>();

        public IEnumerable<ImplementationInfo.ImplementationInfo> GetImplementations(Type type)
        {
            Type collectionType;
            if (type.IsGenericType)
                collectionType = type.GetGenericTypeDefinition();
            else
                collectionType = type;
            if (_dependenciesImplementations.TryGetValue(collectionType,
                out List<ImplementationInfo.ImplementationInfo> dependencyImplementations))
            {
                IEnumerable<ImplementationInfo.ImplementationInfo> result =
                    new List<ImplementationInfo.ImplementationInfo>(dependencyImplementations);
                if (type.IsGenericType)
                    result = result.Where(
                        (impl) => impl.ImplementationType.IsGenericTypeDefinition ||
                        type.IsAssignableFrom(impl.ImplementationType));
                return result;
            }
            else
                return new List<ImplementationInfo.ImplementationInfo>();
        }

        public void Register<TDependency, TImplementation>(bool isSingleton = false, string name = null)
            where TDependency : class
            where TImplementation : TDependency => 
            Register(typeof(TDependency), typeof(TImplementation), isSingleton, name);

        public void Register(Type dependency, Type implementation, bool isSingleton = false, string name = null)
        {
            if (dependency.IsGenericTypeDefinition ^ implementation.IsGenericTypeDefinition)
                throw new ArgumentException("Open generics register should be with both open generic types");
            if (dependency.IsGenericTypeDefinition)
            {
                if (isSingleton)
                    throw new ArgumentException("Open generic cannot be singleton");
                if (!dependency.IsAssignableFromGeneric(implementation))
                    throw new ArgumentException("Dependency is not assignable from implementation");
            }
            else
            {
                if (!dependency.IsClass && !dependency.IsAbstract && !dependency.IsInterface
                    || (!implementation.IsClass || implementation.IsAbstract))
                    throw new ArgumentException("Wrong types");

                if (!dependency.IsAssignableFrom(implementation))
                    throw new ArgumentException("Dependency is not assignable from implementation");
            }
            ImplementationInfo.ImplementationInfo container = 
                new ImplementationInfo.ImplementationInfo(implementation, isSingleton, name);
            if (dependency.IsGenericType)
                dependency = dependency.GetGenericTypeDefinition();
            List<ImplementationInfo.ImplementationInfo> dependencyImplementations;
            lock (_dependenciesImplementations)
            {
                if (!_dependenciesImplementations.TryGetValue(dependency, out dependencyImplementations))
                {
                    dependencyImplementations = new List<ImplementationInfo.ImplementationInfo>();
                    _dependenciesImplementations[dependency] = dependencyImplementations;
                }
            }
            lock (dependencyImplementations)
            {
                if (name != null)
                    dependencyImplementations.RemoveAll((existingContainer) => existingContainer.Name == name);
                dependencyImplementations.Add(container);
            }
        }
    }
}
