using Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class MarshallingTest
    {
        public interface IMockService
        {
            void SetMagicNumber(int input);
            string ReturnString(int input);
        }

        private class MockService : IMockService
        {
            public int MagicNumber { get; private set; }
            public void SetMagicNumber(int input) { MagicNumber = input; }
            public string ReturnString(int input) { return "Hello world " + input + "!"; }
        }

        private MockService _actualService;
        private IMockService _marshalledOnServer;
        private IMockService _marshalledOnClient;

        [SetUp]
        public void Setup()
        {
            _actualService = new MockService();
            _marshalledOnServer = Marshal.Get<IMockService>(_actualService);
            _marshalledOnClient = Marshal.Get<IMockService>((Func<string, object[], object>)
                ((name, args) => ((IMarshalled)_marshalledOnServer).Invoke(name, args)));
        }

        [Test]
        public void TestReturnValue()
        {
            Assert.AreEqual("Hello world 42!", _marshalledOnClient.ReturnString(42));
        }

        [Test]
        public void TestNoReturnValue()
        {
            _marshalledOnClient.SetMagicNumber(69);
            Assert.AreEqual(69, _actualService.MagicNumber);
        }
    }
}
