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
            Assert.Throws<InvalidOperationException>(_mission.Unoffer);
            Assert.Throws<InvalidOperationException>(_mission.Assign);
            Assert.Throws<InvalidOperationException>(_mission.Complete);

            _mission.Offer();
            Assert.AreEqual(MissionState.Offering, _mission.State);
            Assert.Throws<InvalidOperationException>(_mission.Offer);
            Assert.Throws<InvalidOperationException>(_mission.Complete);

            _mission.Assign();
            Assert.AreEqual(MissionState.Assigned, _mission.State);
            Assert.Throws<InvalidOperationException>(_mission.Offer);
            Assert.Throws<InvalidOperationException>(_mission.Unoffer);
            Assert.Throws<InvalidOperationException>(_mission.Assign);

            _mission.Complete();
            Assert.AreEqual(MissionState.Completed, _mission.State);
            Assert.Throws<InvalidOperationException>(_mission.Offer);
            Assert.Throws<InvalidOperationException>(_mission.Unoffer);
            Assert.Throws<InvalidOperationException>(_mission.Assign);
            Assert.Throws<InvalidOperationException>(_mission.Complete);
        }

        [Test]
        public void TestStateTransition_Unoffer()
        {
            _mission.Offer();
            _mission.Unoffer();
            Assert.AreEqual(MissionState.Open, _mission.State);
        }
    }
}
