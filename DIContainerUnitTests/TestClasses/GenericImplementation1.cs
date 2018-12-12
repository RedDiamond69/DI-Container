using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainerUnitTests.TestClasses
{
    public class GenericImplementation1<T> : IGenericInterface<T>
        where T : ISimpleInterface
    {
        internal T _field;

        public GenericImplementation1(T dep)
        {
            _field = dep;
        }
    }
}
