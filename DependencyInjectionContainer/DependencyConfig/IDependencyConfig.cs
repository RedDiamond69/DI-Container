using System;
using System.Collections.Generic;

namespace DependencyInjectionContainer.DependencyConfig
{
    public interface IDependencyConfig
    {
        IEnumerable<ImplementationInfo.ImplementationInfo> GetImplementations(Type type);

        void Register(Type dependency, Type implementation, bool isSingleton = false, string name = null);

        void Register<TDependency, TImplementation>(bool isSingleton = false, string name = null)
            where TDependency : class
            where TImplementation : TDependency;
    }
}
