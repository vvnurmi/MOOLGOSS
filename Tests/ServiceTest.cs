using Axiom.Math;
using Core;
using Core.Items;
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
    public class ServiceTest
    {
        private World _world;
        private World _world2;
        private Service _service;
        private IService PublicService { get { return _service; } }

        [SetUp]
        public void Setup()
        {
            _world = new World();
            _world2 = new World();
            _service = new Service(_world);
        }

        [Test]
        public void TestPatch()
        {
            var clientID = Guid.NewGuid();
            _world2.SetPlanet(new Planet(Guid.NewGuid(), "Earth"));
            PublicService.SendWorldPatch(clientID, new WorldDiff(_world, _world2));
            Assertions.WorldsEqual(_world2, _world);
            var diffIn = PublicService.ReceiveWorldPatch(clientID);
            Assert.True(diffIn.IsEmpty);
        }

        [Test]
        public void TestPatch_TwoClients()
        {
            var world3 = _world.Clone();
            var clientID = Guid.NewGuid();
            var clientID2 = Guid.NewGuid();
            _world2.SetPlanet(new Planet(Guid.NewGuid(), "Earth"));
            PublicService.SendWorldPatch(clientID, new WorldDiff(_world, _world2));
            Assertions.WorldsEqual(_world2, _world);
            var diffIn2 = PublicService.ReceiveWorldPatch(clientID2);
            world3.Patch(diffIn2);
            Assertions.WorldsEqual(_world, world3);
            var diffIn2b = PublicService.ReceiveWorldPatch(clientID2);
            Assert.True(diffIn2b.IsEmpty);
        }
    }
}
