using DependencyInjectionContainer;
using DependencyInjectionContainer.DependencyConfig;
using DIContainerUnitTests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DIContainerUnitTests
{
    [TestClass]
    public class DIContainerUnitTests
    {
        private IDependencyConfig _dependencyConfig;
        private IDependencyProvider _dependencyProvider;

        [TestInitialize]
        public void TestInitialize() => _dependencyConfig = new DependencyConfig();

        [TestMethod]
        public void NonGenericsTypeRegisterTest()
        {
            _dependencyConfig.Register<ISimpleInterface, SimpleImplementation1>();
            _dependencyConfig.Register<ISimpleInterface, SimpleImplementation2>();
            var registeredImplementations = _dependencyConfig.GetImplementations(typeof(ISimpleInterface)).ToList();
            Assert.AreEqual(2, registeredImplementations.Count);
            List<Type> expectedRegisteredTypes = new List<Type>
            {
                typeof(SimpleImplementation1),
                typeof(SimpleImplementation2)
            };
            CollectionAssert.AreEquivalent(expectedRegisteredTypes,
                registeredImplementations.Select((implementation) => implementation.ImplementationType).ToList());
        }

        [TestMethod]
        public void GenericsTypeRegisterTest()
        {
            _dependencyConfig.Register<IGenericInterface<ISimpleInterface>, GenericImplementation1<ISimpleInterface>>();
            _dependencyConfig.Register<IGenericInterface<ISimpleInterface>, GenericImplementation2<ISimpleInterface>>();
            var registeredImplementations = _dependencyConfig.GetImplementations(typeof(IGenericInterface<ISimpleInterface>))
                .ToList();
            Assert.AreEqual(2, registeredImplementations.Count);
            List<Type> expectedRegisteredTypes = new List<Type>
            {
                typeof(GenericImplementation1<ISimpleInterface>),
                typeof(GenericImplementation2<ISimpleInterface>)
            };
            CollectionAssert.AreEquivalent(expectedRegisteredTypes,
                registeredImplementations.Select((implementation) => implementation.ImplementationType).ToList());
        }

        [TestMethod]
        public void OpenGenericsTypeRegisterTest()
        {
            _dependencyConfig.Register(typeof(IGenericInterface<>), typeof(GenericImplementation1<>));
            _dependencyConfig.Register(typeof(IGenericInterface<>), typeof(GenericImplementation2<>));
            var registeredImplementations = _dependencyConfig.GetImplementations(typeof(IGenericInterface<>)).ToList();
            Assert.AreEqual(2, registeredImplementations.Count);
            List<Type> expectedRegisteredTypes = new List<Type>
            {
                typeof(GenericImplementation1<>),
                typeof(GenericImplementation2<>)
            };
            CollectionAssert.AreEquivalent(expectedRegisteredTypes,
                registeredImplementations.Select((implementation) => implementation.ImplementationType).ToList());
        }

        [TestMethod]
        public void NonGenericsTypeResolveTest()
        {
            _dependencyConfig.Register<ISimpleInterface, SimpleImplementation1>();
            _dependencyConfig.Register<ISimpleInterface, SimpleImplementation2>();
            _dependencyProvider = new DependencyProvider(_dependencyConfig);
            var instances = _dependencyProvider.Resolve<ISimpleInterface>();
            Assert.AreEqual(2, instances.Count());
            List<Type> expectedInstancesTypes = new List<Type>
            {
                typeof(SimpleImplementation1),
                typeof(SimpleImplementation2)
            };
            CollectionAssert.AreEquivalent(expectedInstancesTypes,
                instances.Select((instance) => instance.GetType()).ToList());
        }

        [TestMethod]
        public void GenericsTypeResolveTest()
        {
            _dependencyConfig.Register<IGenericInterface<ISimpleInterface>, GenericImplementation1<ISimpleInterface>>();
            _dependencyConfig.Register<IGenericInterface<ISimpleInterface>, GenericImplementation2<ISimpleInterface>>();
            _dependencyProvider = new DependencyProvider(_dependencyConfig);
            var instances = _dependencyProvider.Resolve<IGenericInterface<ISimpleInterface>>();
            Assert.AreEqual(2, instances.Count());
            List<Type> expectedInstancesTypes = new List<Type>
            {
                typeof(GenericImplementation1<ISimpleInterface>),
                typeof(GenericImplementation2<ISimpleInterface>)
            };
            CollectionAssert.AreEquivalent(expectedInstancesTypes,
                instances.Select((instance) => instance.GetType()).ToList());
        }

        [TestMethod]
        public void OpenGenericsTypeResolveTest()
        {
            _dependencyConfig.Register<ISimpleInterface, SimpleImplementation1>();
            _dependencyConfig.Register(typeof(IGenericInterface<>), typeof(GenericImplementation1<>));
            _dependencyConfig.Register(typeof(IGenericInterface<>), typeof(GenericImplementation2<>));
            _dependencyProvider = new DependencyProvider(_dependencyConfig);
            var instances = _dependencyProvider.Resolve<IGenericInterface<ISimpleInterface>>();
            Assert.AreEqual(2, instances.Count());
            List<Type> expectedInstancesTypes = new List<Type>
            {
                typeof(GenericImplementation1<ISimpleInterface>),
                typeof(GenericImplementation2<ISimpleInterface>)
            };
            CollectionAssert.AreEquivalent(expectedInstancesTypes,
                instances.Select((instance) => instance.GetType()).ToList());
            Assert.AreEqual(typeof(SimpleImplementation1),
                instances.OfType<GenericImplementation1<ISimpleInterface>>().First()._field.GetType());
        }

        [TestMethod]
        public void SingletonResolveTest()
        {
            _dependencyConfig.Register<ISimpleInterface, SimpleImplementation1>(true);
            _dependencyProvider = new DependencyProvider(_dependencyConfig);
            Assert.AreSame(_dependencyProvider.Resolve<ISimpleInterface>().First(), 
                _dependencyProvider.Resolve<ISimpleInterface>().First());
        }

        [TestMethod]
        public void ExplicitNameResolveTest()
        {
            _dependencyConfig.Register<ISimpleInterface, SimpleImplementation1>(name: "1");
            _dependencyConfig.Register<ISimpleInterface, SimpleImplementation2>(name: "2");
            _dependencyProvider = new DependencyProvider(_dependencyConfig);
            IEnumerable<ISimpleInterface> instances;
            instances = _dependencyProvider.Resolve<ISimpleInterface>("1");
            Assert.AreEqual(1, instances.Count());
            Assert.AreEqual(typeof(SimpleImplementation1), instances.First().GetType());
            instances = _dependencyProvider.Resolve<ISimpleInterface>("2");
            Assert.AreEqual(1, instances.Count());
            Assert.AreEqual(typeof(SimpleImplementation2), instances.First().GetType());
        }

        [TestMethod]
        public void ConstructorNameResolveTest()
        {
            _dependencyConfig.Register<ISimpleInterface, SimpleImplementation1>(name: "1");
            _dependencyConfig.Register<ISimpleInterface, SimpleImplementation2>(name: "2");
            _dependencyConfig.Register<ISimpleInterface, ConstructorParameterImplementation>();
            _dependencyProvider = new DependencyProvider(_dependencyConfig);
            var instances = _dependencyProvider.Resolve<ISimpleInterface>().OfType<ConstructorParameterImplementation>();
            Assert.AreEqual(1, instances.Count());
            Assert.AreEqual(typeof(SimpleImplementation1), instances.First()._intfImpl1.GetType());
            Assert.AreEqual(typeof(SimpleImplementation2), instances.First()._intfImpl2.GetType());
        }
    }
}
