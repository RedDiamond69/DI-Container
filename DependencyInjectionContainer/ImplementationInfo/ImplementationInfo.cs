using System;

namespace DependencyInjectionContainer.ImplementationInfo
{
    public class ImplementationInfo
    {
        public Type ImplementationType { get; }
        public bool IsSingleton { get; }
        public object SingletonInstance { get; set; }
        public string Name { get; }

        public ImplementationInfo(Type type, bool isSingleton, string name)
        {
            ImplementationType = type;
            Name = name;
            SingletonInstance = null;
            IsSingleton = isSingleton;
        }
    }
}
