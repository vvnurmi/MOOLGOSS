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
        private Planet _planet;
        private Station _station;
        private Ship _ship;
        private Inventory _inventory;

        [SetUp]
        public void Setup()
        {
            _planet = new Planet(Guid.NewGuid(), "Earth");
            _station = new Station(Guid.NewGuid(), new Vector3(10, 0, 20));
            _ship = new Ship(Guid.NewGuid(), new Vector3(5, 6, 7), Vector3.UnitX, Vector3.UnitY);
            _inventory = new Inventory(Guid.NewGuid());
        }

        [Test]
        public void IsEmptyAfterCreation()
        {
            CollectionAssert.IsEmpty(World.Empty.Planets);
            CollectionAssert.IsEmpty(World.Empty.Stations);
            CollectionAssert.IsEmpty(World.Empty.Ships);
            CollectionAssert.IsEmpty(World.Empty.Inventories);
        }

        [Test]
        public void SetItems()
        {
            var world = GetWorldWithItems();
            CollectionAssert.AreEquivalent(new[] { _planet }, world.Planets.Values);
            CollectionAssert.AreEquivalent(new[] { _station }, world.Stations.Values);
            CollectionAssert.AreEquivalent(new[] { _ship }, world.Ships.Values);
            CollectionAssert.AreEquivalent(new[] { _inventory }, world.Inventories.Values);
        }

        [Test]
        public void SetNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => World.Empty.SetPlanet(null));
            Assert.Throws<ArgumentNullException>(() => World.Empty.SetStation(null));
            Assert.Throws<ArgumentNullException>(() => World.Empty.SetShip(null));
            Assert.Throws<ArgumentNullException>(() => World.Empty.SetInventory(null));
        }

        [Test]
        public void RemovingNonexistent_DoesntThrow()
        {
            World.Empty.RemovePlanet(_planet.ID);
            World.Empty.RemoveStation(_station.ID);
            World.Empty.RemoveShip(_ship.ID);
            World.Empty.RemoveInventory(_inventory.ID);
        }

        [Test]
        public void RemoveItems()
        {
            var world = GetWorldWithItems();
            world = world.RemovePlanet(_planet.ID);
            CollectionAssert.IsEmpty(world.Planets);
            world = world.RemoveStation(_station.ID);
            CollectionAssert.IsEmpty(world.Stations);
            world = world.RemoveShip(_ship.ID);
            CollectionAssert.IsEmpty(world.Ships);
            world = world.RemoveInventory(_inventory.ID);
            CollectionAssert.IsEmpty(world.Inventories);
        }

        [Test]
        public void GetItems()
        {
            Assert.IsNull(World.Empty.GetPlanet(_planet.ID));
            Assert.IsNull(World.Empty.GetStation(_station.ID));
            Assert.IsNull(World.Empty.GetShip(_ship.ID));
            Assert.IsNull(World.Empty.GetInventory(_inventory.ID));
            var world = GetWorldWithItems();
            Assert.AreEqual(_planet, world.GetPlanet(_planet.ID));
            Assert.AreEqual(_station, world.GetStation(_station.ID));
            Assert.AreEqual(_ship, world.GetShip(_ship.ID));
            Assert.AreEqual(_inventory, world.GetInventory(_inventory.ID));
        }

        private World GetWorldWithItems()
        {
            return World.Empty
                .SetPlanet(_planet)
                .SetStation(_station)
                .SetShip(_ship)
                .SetInventory(_inventory);
        }
    }
}
