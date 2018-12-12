using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DependencyInjectionContainer.ImplementationInfo;

namespace DependencyInjectionContainer.DependencyConfig
{
    public class DependencyConfig : IDependencyConfig
    {
        public DependencyConfig()
        {
        }

        public IEnumerable<ImplementationInfo.ImplementationInfo> GetImplementations(Type type)
        {
            throw new NotImplementedException();
        }

        public void Register(Type dependency, Type implementation, bool isSingleton = false, string name = null)
        {
            throw new NotImplementedException();
        }

        public void Register<TDependency, TImplementation>(bool isSingleton = false, string name = null)
            where TDependency : class
            where TImplementation : TDependency
        {
            throw new NotImplementedException();
        }
    }
}
