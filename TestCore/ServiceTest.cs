using Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCore
{
    public interface ITestService
    {
        string Hello();
    }

    internal class TestService : ITestService
    {
        public string Hello()
        {
            return "Hello Test!";
        }
    }

    [TestFixture]
    public class ServiceTest
    {
        [Test]
        public void TestServiceCreation()
        {
            var service = new TestService();
            ITransport transport = new InstanceTransport(service);
            var factory = new ServiceFactory(transport);
            ITestService serviceProxy = factory.GetProxy<ITestService>();
            Assert.AreEqual("Hello Test!", serviceProxy.Hello());
        }
    }
}
