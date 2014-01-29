using Axiom.Math;
using Client;
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
    public class MissionTest
    {
        private Mission _mission;

        [SetUp]
        public void Setup()
        {
            _mission = new Mission
            {
                AssignMessage = "Go and find a planet! There'll be no reward.",
                AssignVolume = new Sphere(new Vector3(100, 0, 200), 60),
                CompleteMessage = "You found the planet, nice!",
                CompleteVolume = new Sphere(new Vector3(200, 0, 200), 30),
            };
        }

        [Test]
        public void TestStateTransition()
        {
            Assert.AreEqual(MissionState.Open, _mission.State);
            AssertInvalidOperations(_mission.Suppress, _mission.Assign, _mission.Complete);

            _mission.Offer();
            Assert.AreEqual(MissionState.Offering, _mission.State);
            AssertInvalidOperations(_mission.Offer, _mission.Complete);

            _mission.Assign();
            Assert.AreEqual(MissionState.Assigned, _mission.State);
            AssertInvalidOperations(_mission.Offer, _mission.Suppress, _mission.Assign);

            _mission.Complete();
            Assert.AreEqual(MissionState.Completed, _mission.State);
            AssertInvalidOperations(_mission.Offer, _mission.Suppress, _mission.Assign, _mission.Complete);
        }

        [Test]
        public void TestStateTransition_Suppress()
        {
            _mission.Offer();
            _mission.Suppress();
            Assert.AreEqual(MissionState.Suppressed, _mission.State);
            AssertInvalidOperations(_mission.Suppress, _mission.Assign, _mission.Complete);

            _mission.Offer();
            Assert.AreEqual(MissionState.Offering, _mission.State);
        }

        private void AssertInvalidOperations(params TestDelegate[] ops)
        {
            foreach (var op in ops)
                Assert.Throws<InvalidOperationException>(op);
        }
    }
}
