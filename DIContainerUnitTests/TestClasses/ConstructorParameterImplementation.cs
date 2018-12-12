using DependencyInjectionContainer.DependencyAttributes;

namespace DIContainerUnitTests.TestClasses
{
    public class ConstructorParameterImplementation : ISimpleInterface
    {
        internal readonly ISimpleInterface _intfImpl1;
        internal readonly ISimpleInterface _intfImpl2;

        public ConstructorParameterImplementation([Dependency("1")] ISimpleInterface intfImpl1,
            [Dependency("2")] ISimpleInterface intfImpl2)
        {
            _intfImpl1 = intfImpl1;
            _intfImpl2 = intfImpl2;
        }
    }
}
