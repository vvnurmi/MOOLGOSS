using Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class SerializationTest
    {
        [Serializable]
        private class MockData
        {
            public string Text { get; set; }
            public int Number { get; set; }
        }

        [Test]
        public void TestString()
        {
            byte[] data = Serialization.Break("Foobar");
            Assert.AreEqual("Foobar", Serialization.Build<string>(data));
        }

        [Test]
        public void TestClass()
        {
            byte[] data = Serialization.Break(new MockData { Text = "Hello", Number = 42 });
            var rebuilt = Serialization.Build<MockData>(data);
            Assert.AreEqual("Hello", rebuilt.Text);
            Assert.AreEqual(42, rebuilt.Number);
        }
    }
}
