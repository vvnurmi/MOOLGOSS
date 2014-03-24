using Axiom.Math;
using Core;
using Core.Wobs;
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
        [Test]
        public void TestCreation()
        {
            var ship = new Ship(Guid.NewGuid(), new Pose(new Vector3(2, 3, 4), Vector3.UnitX, Vector3.UnitY));
            Assert.AreEqual(new Vector3(2, 3, 4), ship.Pose.Location);
            Assert.AreEqual(new Vector3(1, 0, 0), ship.Pose.Front);
            Assert.AreEqual(new Vector3(0, 1, 0), ship.Pose.Up);
            Assert.AreEqual(new Vector3(0, 0, 1), ship.Pose.Right);
        }
    }
}
