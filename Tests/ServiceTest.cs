using Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public interface ITestService
    {
        string Hello();
        void Set(int value);
        int Get();
    }

    internal class TestService : ITestService
    {
        private int _value;

        public string Hello()
        {
            return "Hello Test!";
        }

        public void Set(int value)
        {
            _value = value;
        }

        public int Get()
        {
            return _value;
        }
    }

    [TestFixture]
    public class ServiceTest
    {
        [Test]
        public void TestServiceProxy()
        {
            object service = new TestService();
            ITransport transport = new InstanceTransport(service);
            var factory = new ServiceFactory(transport);
            ITestService serviceProxy = factory.GetProxy<ITestService>();
            Assert.AreEqual("Hello Test!", serviceProxy.Hello());
        }

        [Test]
        public void TestTransport()
        {
            var service = new TestService();
            var transport = new InstanceTransport(service);
            transport.InvokeAction("Set", 42);
            Assert.AreEqual(42, service.Get());
        }
    }
}
