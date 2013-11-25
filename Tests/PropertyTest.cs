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
            var nameString1 = new Property<string>("name");
            var nameString2 = new Property<string>("name");
            var nameInt = new Property<int>("name");
            var sizeInt = new Property<int>("size");
            Assert.AreEqual(nameString1, nameString2);
            Assert.AreNotEqual(nameString1, nameInt);
            Assert.AreNotEqual(nameInt, sizeInt);
        }

        [Test]
        public void TestMissingProperty()
        {
            var prop = new Property<string>("foo");
            var entity = new TestEntity("shenanigan");
            Assert.Throws<PropertyException>(() => entity.GetValue(prop));
        }

        [Test]
        public void TestGetValue()
        {
            var prop = new Property<string>("Name");
            var entity = new TestEntity("shenanigan");
            Assert.AreEqual("shenanigan", entity.GetValue(prop));
        }
    }
}
