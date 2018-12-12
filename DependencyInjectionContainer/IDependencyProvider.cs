using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public interface IDependencyProvider
    {
        IEnumerable<TDependency> Resolve<TDependency>(string name = null)
            where TDependency : class;
    }
}
