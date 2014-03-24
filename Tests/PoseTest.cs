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
    public class PoseTest
    {
        [Test]
        public void TestMove()
        {
            var pose = new Pose(new Vector3(2, 3, 4), Vector3.UnitX, Vector3.UnitY);
            pose = pose.Move(pose.Front, 0, 0, 0);
            Assert.AreEqual(new Vector3(3, 3, 4), pose.Location);
            pose = pose.Move(Vector3.Zero, 90, 0, 0);
            Assert.AreEqual(Tuple.Create(Vector3.UnitY, -Vector3.UnitX), Tuple.Create(pose.Front, pose.Up));
            pose = pose.Move(Vector3.Zero, 0, 90, 0);
            Assert.AreEqual(Tuple.Create(-Vector3.UnitZ, -Vector3.UnitX), Tuple.Create(pose.Front, pose.Up));
        }
    }
}
