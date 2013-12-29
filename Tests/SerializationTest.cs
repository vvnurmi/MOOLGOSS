using Axiom.Math;
using Core;
using Core.Serial;
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

        [Test]
        public void TestArray()
        {
            byte[] data = Serialization.Break(new[] { 42, 69, 99 });
            CollectionAssert.AreEqual(new[] { 42, 69, 99 }, Serialization.Build<int[]>(data));
        }

        [Test]
        public void TestThirdPartyTypes()
        {
            byte[] data = Serialization.Break(new Vector3(2, 3, 4));
            Assert.AreEqual(new Vector3(2, 3, 4), Serialization.Build<Vector3>(data));
        }

        [Test, Timeout(1000)]
        public void TestSpeed()
        {
            int count = 100000;
            var start = DateTime.Now;
            for (int i = 0; i < count; i++)
                Serialization.Build<string>(Serialization.Break("Hello world!"));
            var end = DateTime.Now;
            Console.WriteLine("{0} repetitions of simple string round-trip serialization took {1} seconds",
                count, (end - start).TotalSeconds);
        }
    }
}
