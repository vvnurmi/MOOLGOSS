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
        private World _serverWorld;
        private Service _service;
        private IService PublicService { get { return _service; } }

        [SetUp]
        public void Setup()
        {
            _serverWorld = World.Empty;
            _service = new Service(() => _serverWorld, f => _serverWorld = f(_serverWorld));
        }

        [Test]
        public void TestPatch()
        {
            var clientID = Guid.NewGuid();
            var clientWorld = World.Empty.SetWob(new Planet(Guid.NewGuid(), "Earth"));
            PublicService.SendWorldPatch(clientID, new WorldDiff(_serverWorld, clientWorld));
            Assertions.WorldsEqual(clientWorld, _serverWorld);
            var diffIn = PublicService.ReceiveWorldPatch(clientID);
            Assert.True(diffIn.IsEmpty);
        }

        [Test]
        public void TestPatch_TwoClients()
        {
            var client1ID = Guid.NewGuid();
            var client2ID = Guid.NewGuid();
            var client1World = World.Empty.SetWob(new Planet(Guid.NewGuid(), "Earth"));
            var client2World = World.Empty;
            PublicService.SendWorldPatch(client1ID, new WorldDiff(_serverWorld, client1World));
            Assertions.WorldsEqual(client1World, _serverWorld);
            var diffIn2 = PublicService.ReceiveWorldPatch(client2ID);
            client2World = client2World.Patch(diffIn2);
            Assertions.WorldsEqual(_serverWorld, client2World);
            var diffIn2b = PublicService.ReceiveWorldPatch(client2ID);
            Assert.True(diffIn2b.IsEmpty);
        }
    }
}
