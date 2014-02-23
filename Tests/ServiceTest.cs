﻿using Axiom.Math;
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
            _world2.AddPlanet(new Planet(Guid.NewGuid(), "Earth"));
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
            _world2.AddPlanet(new Planet(Guid.NewGuid(), "Earth"));
            PublicService.SendWorldPatch(clientID, new WorldDiff(_world, _world2));
            Assertions.WorldsEqual(_world2, _world);
            var diffIn2 = PublicService.ReceiveWorldPatch(clientID2);
            world3.Patch(diffIn2);
            Assertions.WorldsEqual(_world, world3);
            var diffIn2b = PublicService.ReceiveWorldPatch(clientID2);
            Assert.True(diffIn2b.IsEmpty);
        }

        [Test]
        public void TestGetPlanets()
        {
            _world.AddPlanet(new Planet(Guid.NewGuid(), "Earth"));
            CollectionAssert.AreEquivalent(new[] { "Earth" }, PublicService.GetPlanets().Select(x => x.Name));
        }

        [Test]
        public void TestGetStations()
        {
            _service.AddStation(Guid.NewGuid(), new Vector3(100, 0, 200));
            _service.AddStation(Guid.NewGuid(), new Vector3(300, 0, 100));
            CollectionAssert.AreEqual(
                new[] { new Vector3(100, 0, 200), new Vector3(300, 0, 100) },
                PublicService.GetStations().Select(x => x.Pos));
        }

        [Test]
        public void TestShips()
        {
            CollectionAssert.IsEmpty(PublicService.GetShips());
            var id = Guid.NewGuid();
            Action<Vector3, Vector3, Vector3> setAndAssertShip = (pos, front, up) =>
            {
                PublicService.UpdateShip(id, pos, front, up);
                Ship ship = PublicService.GetShips().SingleOrDefault();
                Assert.IsNotNull(ship);
                var expected = Tuple.Create(id, pos, front, up);
                Assert.AreEqual(expected, Tuple.Create(ship.ID, ship.Pos, ship.Front, ship.Up));
            };
            setAndAssertShip(new Vector3(1, 2, 3), Vector3.UnitX, Vector3.UnitY);
            setAndAssertShip(new Vector3(2, 2, 3), Vector3.UnitY, Vector3.UnitZ);
        }

        [Test]
        public void TestInventoryAdd_Success()
        {
            var id = Guid.NewGuid();
            Assert.IsEmpty(PublicService.GetInventory(id));
            PublicService.AddToInventory(id, new ItemStack(Guid.NewGuid(), ItemType.MiningDroid, 1));
        }

        [Test]
        public void TestInventoryAdd_DoubleAddFails()
        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var stack = new ItemStack(Guid.NewGuid(), ItemType.MiningDroid, 1);
            PublicService.AddToInventory(id1, stack);
            PublicService.AddToInventory(id1, stack); // Fails silently.
            PublicService.AddToInventory(id2, stack); // Fails silently.
            Assert.AreEqual(1, PublicService.GetInventory(id1).Count());
            Assert.AreEqual(0, PublicService.GetInventory(id2).Count());
        }
    }
}
