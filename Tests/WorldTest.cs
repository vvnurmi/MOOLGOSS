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
            _station = new Station(Guid.NewGuid());
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
        public void AddItems()
        {
            _world.AddPlanet(_planet);
            _world.AddStation(_station);
            _world.AddShip(_ship);
            _world.AddInventory(_inventory);
            CollectionAssert.AreEquivalent(new[] { _planet }, _world.Planets.Values);
            CollectionAssert.AreEquivalent(new[] { _station }, _world.Stations.Values);
            CollectionAssert.AreEquivalent(new[] { _ship }, _world.Ships.Values);
            CollectionAssert.AreEquivalent(new[] { _inventory }, _world.Inventories.Values);
        }

        [Test]
        public void AddNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _world.AddPlanet(null));
            Assert.Throws<ArgumentNullException>(() => _world.AddStation(null));
            Assert.Throws<ArgumentNullException>(() => _world.AddShip(null));
            Assert.Throws<ArgumentNullException>(() => _world.AddInventory(null));
        }

        [Test]
        public void RemoveItems()
        {
            Assert.False(_world.RemovePlanet(_planet.ID));
            Assert.False(_world.RemoveStation(_station.ID));
            Assert.False(_world.RemoveShip(_ship.ID));
            Assert.False(_world.RemoveInventory(_inventory.ID));
            AddItems();
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
        public void DoubleAdd_Throws()
        {
            _world.AddPlanet(_planet);
            Assert.Throws<ArgumentException>(() => _world.AddPlanet(_planet));
        }

        [Test]
        public void GetItems()
        {
            Assert.IsNull(_world.GetPlanet(_planet.ID));
            Assert.IsNull(_world.GetStation(_station.ID));
            Assert.IsNull(_world.GetShip(_ship.ID));
            Assert.IsNull(_world.GetInventory(_inventory.ID));
            AddItems();
            Assert.AreEqual(_planet, _world.GetPlanet(_planet.ID));
            Assert.AreEqual(_station, _world.GetStation(_station.ID));
            Assert.AreEqual(_ship, _world.GetShip(_ship.ID));
            Assert.AreEqual(_inventory, _world.GetInventory(_inventory.ID));
        }
    }
}
