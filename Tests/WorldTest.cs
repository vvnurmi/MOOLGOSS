using Axiom.Math;
using Core;
using Core.Items;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class WorldTest
    {
        private World _world;
        private Planet _planet;
        private Station _station;
        private Ship _ship;
        private Inventory _inventory;

        [SetUp]
        public void Setup()
        {
            _world = new World();
            _planet = new Planet(Guid.NewGuid(), "Earth");
            _station = new Station(Guid.NewGuid(), new Vector3(10, 0, 20));
            _ship = new Ship(Guid.NewGuid(), new Vector3(5, 6, 7), Vector3.UnitX, Vector3.UnitY);
            _inventory = new Inventory(Guid.NewGuid());
        }

        [Test]
        public void IsEmptyAfterCreation()
        {
            CollectionAssert.IsEmpty(_world.Planets);
            CollectionAssert.IsEmpty(_world.Stations);
            CollectionAssert.IsEmpty(_world.Ships);
            CollectionAssert.IsEmpty(_world.Inventories);
        }

        [Test]
        public void CloneIsEqual()
        {
            SetItemsTo(_world);
            Assertions.WorldsEqual(_world, _world.Clone());
        }

        [Test]
        public void CloneIsNotSelf()
        {
            Assert.AreNotSame(_world, _world.Clone());
        }

        [Test]
        public void SetItems()
        {
            SetItemsTo(_world);
            CollectionAssert.AreEquivalent(new[] { _planet }, _world.Planets.Values);
            CollectionAssert.AreEquivalent(new[] { _station }, _world.Stations.Values);
            CollectionAssert.AreEquivalent(new[] { _ship }, _world.Ships.Values);
            CollectionAssert.AreEquivalent(new[] { _inventory }, _world.Inventories.Values);
        }

        [Test]
        public void SetNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _world.SetPlanet(null));
            Assert.Throws<ArgumentNullException>(() => _world.SetStation(null));
            Assert.Throws<ArgumentNullException>(() => _world.SetShip(null));
            Assert.Throws<ArgumentNullException>(() => _world.SetInventory(null));
        }

        [Test]
        public void RemoveItems()
        {
            Assert.False(_world.RemovePlanet(_planet.ID));
            Assert.False(_world.RemoveStation(_station.ID));
            Assert.False(_world.RemoveShip(_ship.ID));
            Assert.False(_world.RemoveInventory(_inventory.ID));
            SetItemsTo(_world);
            Assert.True(_world.RemovePlanet(_planet.ID));
            Assert.True(_world.RemoveStation(_station.ID));
            Assert.True(_world.RemoveShip(_ship.ID));
            Assert.True(_world.RemoveInventory(_inventory.ID));
            CollectionAssert.IsEmpty(_world.Planets);
            CollectionAssert.IsEmpty(_world.Stations);
            CollectionAssert.IsEmpty(_world.Ships);
            CollectionAssert.IsEmpty(_world.Inventories);
        }

        [Test]
        public void GetItems()
        {
            Assert.IsNull(_world.GetPlanet(_planet.ID));
            Assert.IsNull(_world.GetStation(_station.ID));
            Assert.IsNull(_world.GetShip(_ship.ID));
            Assert.IsNull(_world.GetInventory(_inventory.ID));
            SetItemsTo(_world);
            Assert.AreEqual(_planet, _world.GetPlanet(_planet.ID));
            Assert.AreEqual(_station, _world.GetStation(_station.ID));
            Assert.AreEqual(_ship, _world.GetShip(_ship.ID));
            Assert.AreEqual(_inventory, _world.GetInventory(_inventory.ID));
        }

        private void SetItemsTo(World world)
        {
            world.SetPlanet(_planet);
            world.SetStation(_station);
            world.SetShip(_ship);
            world.SetInventory(_inventory);
        }
    }
}
