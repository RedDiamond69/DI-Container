using DependencyInjectionContainer.DependencyConfig;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public class DependencyProvider : IDependencyProvider
    {
        private readonly IDependencyConfig _dependencyConfig;
        private readonly ConcurrentDictionary<int, Stack<Type>> _recursionExcludeTypes;

        public DependencyProvider()
        {
        }

        public IEnumerable<TDependency> Resolve<TDependency>(string name = null) where TDependency : class
        {
            throw new NotImplementedException();
        }
    }
}
