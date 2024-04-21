using DependencyInjectionLib;
using static TestProject.TestClasses;
using System.Collections.Generic;
using System.Net.NetworkInformation;


namespace TestProject
{
    public class LibraryTests
    {
        private Configuration _configuration;

        [SetUp]
        public void Setup()
        {
            _configuration = new Configuration();
        }

        [Test]
        public void ResolveBasicTypes()
        {
            _configuration.Register<IService, Service1>();
            _configuration.Register<AbstractService, AbstractServiceImpl>();

            Provider provider = new Provider(_configuration);
            object service1 = provider.Resolve<IService>();
            object abstractServiceImpl = provider.Resolve<AbstractService>();

            Assert.AreEqual(typeof(Service1), service1.GetType());
            Assert.AreEqual(typeof(AbstractServiceImpl), abstractServiceImpl.GetType());
        }

        [Test]
        public void ResolveRecursiveDependency()
        {
            _configuration.Register<IService, Service3>();
            _configuration.Register<IRepository, Repository1>();

            Provider provider = new Provider(_configuration);
            object service = provider.Resolve<IService>();

            Assert.AreEqual(typeof(Service3), service.GetType());
        }

        [Test]
        public void ResolveInstancePerDependencyCreating()
        {
            _configuration.Register<IService, Service1>(false);

            Provider provider = new Provider(_configuration);
            object service1 = provider.Resolve<IService>();
            object service2 = provider.Resolve<IService>();

            Assert.AreNotEqual(service1, service2);
        }

        [Test]
        public void ResolveEnumerable()
        {
            _configuration.Register<IService, Service1>();
            _configuration.Register<IService, Service2>();
            _configuration.Register<IService, Service3>();
            _configuration.Register<IRepository, Repository1>();

            Provider provider = new Provider(_configuration);
            List<object> services = provider.Resolve<IEnumerable<IService>>() as List<object>;

            Assert.AreEqual(3, services?.Count);
            Assert.AreEqual(typeof(Service1), services[0].GetType());
            Assert.AreEqual(typeof(Service2), services[1].GetType());
            Assert.AreEqual(typeof(Service3), services[2].GetType());
        }

        [Test]
        public void ResolveGenericDependency()
        {
            _configuration.Register<IRepository, Repository1>();
            _configuration.Register<IService<IRepository>, Service4<IRepository>>();

            Provider provider = new Provider(_configuration);
            object service = provider.Resolve<IService<IRepository>>();

            Assert.AreEqual(typeof(Service4<IRepository>), service.GetType());
            Assert.NotNull((service as Service4<IRepository>)?.Repository);
        }

        [Test]
        public void ResolveOpenGenericsDependency()
        {
            _configuration.Register<IRepository, Repository1>();
            _configuration.Register(typeof(IService<>), typeof(Service4<>));

            Provider provider = new Provider(_configuration);
            object service = provider.Resolve<IService<IRepository>>();

            Assert.AreEqual(typeof(Service4<IRepository>), service.GetType());
            Assert.NotNull((service as Service4<IRepository>)?.Repository);
        }



    }
}