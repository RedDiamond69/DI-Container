using DependencyInjectionContainer.DependencyConfig;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace DependencyInjectionContainer
{
    public class DependencyProvider : IDependencyProvider
    {
        private readonly IDependencyConfig _dependencyConfig;
        private readonly ConcurrentDictionary<int, Stack<Type>> _recursionExcludTypes;

        public DependencyProvider(IDependencyConfig dependencyConfig)
        {
            _dependencyConfig = dependencyConfig;
            _recursionExcludTypes = new ConcurrentDictionary<int, Stack<Type>>();
        }

        private object CreateByConstructor(Type t)
        {
            ConstructorInfo[] constructors = t.GetConstructors()
                .OrderBy((constructor) => constructor.GetParameters().Length).ToArray();
            object instance = null;
            List<object> parameters = new List<object>();
            _recursionExcludTypes[Thread.CurrentThread.ManagedThreadId].Push(t);
            for (int constructor = 0; (constructor < constructors.Length) && (instance == null); ++constructor)
            {
                try
                {
                    foreach (ParameterInfo constructorParameter in constructors[constructor].GetParameters())
                    {
                        parameters.Add(Resolve(constructorParameter.ParameterType,
                            constructorParameter.GetCustomAttribute<DependencyAttributes.DependencyAttribute>()?.Name).FirstOrDefault());
                    }
                    instance = constructors[constructor].Invoke(parameters.ToArray());
                }
                catch
                {
                    parameters.Clear();
                }
            }
            _recursionExcludTypes[Thread.CurrentThread.ManagedThreadId].Pop();
            return instance;
        }

        public IEnumerable<TDependency> Resolve<TDependency>(string name = null)
            where TDependency : class
        {
            Type dependencyType = typeof(TDependency);
            if (dependencyType.IsGenericTypeDefinition)
                throw new ArgumentException("Generic type definition resolving is not supproted");
            if (_recursionExcludTypes.TryGetValue(Thread.CurrentThread.ManagedThreadId, out Stack<Type> types))
                types.Clear();
            else
                _recursionExcludTypes[Thread.CurrentThread.ManagedThreadId] = new Stack<Type>();
            return Resolve(dependencyType, name).OfType<TDependency>();
        }

        private IEnumerable<object> Resolve(Type dep, string name)
        {
            if (dep.IsGenericType || dep.IsGenericTypeDefinition)
                return ResolveGenerical(dep, name);
            else
                return ResolveNonGenerical(dep, name);
        }

        private IEnumerable<object> ResolveGenerical(Type dep, string name)
        {
            List<object> result = new List<object>();
            IEnumerable<ImplementationInfo.ImplementationInfo> implementationInformation
                = _dependencyConfig.GetImplementations(dep)
                .Where((impl) => 
                !_recursionExcludTypes[Thread.CurrentThread.ManagedThreadId].Contains(impl.ImplementationType));
            if (name != null)
                implementationInformation = implementationInformation.Where(
                    (container) => container.Name == name);
            object instance = null;
            foreach (ImplementationInfo.ImplementationInfo info in implementationInformation)
            {
                instance = CreateByConstructor(info.ImplementationType.GetGenericTypeDefinition()
                    .MakeGenericType(dep.GenericTypeArguments));
                if (instance != null)
                    result.Add(instance);
            }
            return result;
        }

        private IEnumerable<object> ResolveNonGenerical(Type dep, string name)
        {
            if (dep.IsValueType)
                return new List<object>
                {
                    Activator.CreateInstance(dep)
                };
            IEnumerable<ImplementationInfo.ImplementationInfo> implementationInformation =
                _dependencyConfig.GetImplementations(dep)
                .Where((impl) => 
                !_recursionExcludTypes[Thread.CurrentThread.ManagedThreadId].Contains(impl.ImplementationType));
            if (name != null)
                implementationInformation = implementationInformation
                    .Where((implementation) => implementation.Name == name);
            List<object> result = new List<object>();
            object dependencyInstance = null;
            foreach (ImplementationInfo.ImplementationInfo info in implementationInformation)
            {
                if (info.IsSingleton)
                {
                    if (info.SingletonInstance == null)
                        lock (info)
                            if (info.SingletonInstance == null)
                                info.SingletonInstance = CreateByConstructor(info.ImplementationType);
                    dependencyInstance = info.SingletonInstance;
                }
                else
                    dependencyInstance = CreateByConstructor(info.ImplementationType);
                if (dependencyInstance != null)
                    result.Add(dependencyInstance);
            }
            return result;
        }
    }
}
