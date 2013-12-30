using Axiom.Math;
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
    public class ShipTest
    {
        private Ship _ship;

        [SetUp]
        public void Setup()
        {
            _ship = new Ship(Guid.NewGuid(), new Vector3(2, 3, 4), Vector3.UnitX, Vector3.UnitY);
        }

        [Test]
        public void TestCreation()
        {
            Assert.AreEqual(new Vector3(2, 3, 4), _ship.Pos);
            Assert.AreEqual(new Vector3(1, 0, 0), _ship.Front);
            Assert.AreEqual(new Vector3(0, 1, 0), _ship.Up);
            Assert.AreEqual(new Vector3(0, 0, 1), _ship.Right);
        }

        [Test]
        public void TestMove()
        {
            _ship.Move(_ship.Front);
            Assert.AreEqual(new Vector3(3, 3, 4), _ship.Pos);
            _ship.Pitch(90);
            Assert.AreEqual(Tuple.Create(Vector3.UnitY, -Vector3.UnitX), Tuple.Create(_ship.Front, _ship.Up));
            _ship.Yaw(90);
            Assert.AreEqual(Tuple.Create(-Vector3.UnitZ, -Vector3.UnitX), Tuple.Create(_ship.Front, _ship.Up));
        }
    }
}
