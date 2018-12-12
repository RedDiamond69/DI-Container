using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer.DependencyAttributes
{
    public class DependencyAttribute : Attribute
    {
        public string Name { get; protected set; }

        public DependencyAttribute(string name)
        {
            Name = name;
        }
    }
}
