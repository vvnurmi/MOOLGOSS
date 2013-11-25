using Core;
using NUnit.Framework;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class UniverseTest
    {
        [Test]
        public void TestEntity()
        {
            IEntity planet = new Planet("Earth");
            var nameProp = new Property<string>("Name");
            CollectionAssert.AreEqual(new[] { nameProp }, planet.GetProps());
            Assert.AreEqual("Earth", planet.GetValue(nameProp));
        }

        [Test]
        public void TestUniverse()
        {
            var universe = new Universe();
            var planet = new Planet("Earth");
            universe.Add((IEntity)planet);
            CollectionAssert.AreEquivalent(new[] { planet }, universe.GetPlanets());
        }
    }
}
