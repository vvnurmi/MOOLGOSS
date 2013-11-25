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
    public class PropertyTest
    {
        private class TestEntity : IEntity
        {
            [Prop]
            public string Name { get; private set; }

            public TestEntity(string name)
            {
                Name = name;
            }
        }

        [Test]
        public void TestPropertyEquality()
        {
            var nameString1 = new Prop("name", typeof(string));
            var nameString2 = new Prop("name", typeof(string));
            var nameInt = new Prop("name", typeof(int));
            var sizeInt = new Prop("size", typeof(int));
            Assert.AreEqual(nameString1, nameString2);
            Assert.AreNotEqual(nameString1, nameInt);
            Assert.AreNotEqual(nameInt, sizeInt);
        }

        [Test]
        public void TestMissingProperty()
        {
            var prop = new Prop("foo", typeof(string));
            var entity = new TestEntity("shenanigan");
            Assert.Throws<PropertyException>(() => entity.GetValue(prop));
        }

        [Test]
        public void TestGetValue()
        {
            var prop = new Prop("Name", typeof(string));
            var entity = new TestEntity("shenanigan");
            Assert.AreEqual("shenanigan", entity.GetValue(prop));
        }
    }
}
