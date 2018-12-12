using System;

namespace DependencyInjectionContainer.DependencyAttributes
{
    public class DependencyAttribute : Attribute
    {
        public string Name { get; }

        public DependencyAttribute(string name)
        {
            Name = name;
        }
    }
}
