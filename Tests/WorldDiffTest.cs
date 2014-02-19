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
    public class WorldDiffTest
    {
        private World _world, _world2;
        private Planet _planet;
        private Station _station;
        private Ship _ship;
        private Inventory _inventory;

        [SetUp]
        public void Setup()
        {
            _world = new World();
            _world2 = new World();
            _planet = new Planet(Guid.NewGuid(), "Earth");
            _station = new Station(Guid.NewGuid());
            _ship = new Ship(Guid.NewGuid(), new Vector3(5, 6, 7), Vector3.UnitX, Vector3.UnitY);
            _inventory = new Inventory(Guid.NewGuid());
        }

        [Test]
        public void EmptyForIdenticals()
        {
            _world2.AddPlanet(_planet);
            Assert.True(new WorldDiff(_world, _world).IsEmpty);
            Assert.True(new WorldDiff(_world2, _world2).IsEmpty);
            Assert.False(new WorldDiff(_world, _world2).IsEmpty);
        }

        [Test]
        public void ItemsMatchedByID()
        {
            var planetClone = new Planet(_planet.ID, _planet.Name);
            _world.AddPlanet(_planet);
            _world2.AddPlanet(planetClone);
            Assert.True(new WorldDiff(_world, _world2).IsEmpty);
        }

        [Test]
        public void Added_Planet()
        {
            _world2.AddPlanet(_planet);
            CollectionAssert.AreEquivalent(new[] { _planet }, new WorldDiff(_world, _world2).Planets.Added.Values);
        }

        [Test]
        public void Added_Station()
        {
            _world2.AddStation(_station);
            CollectionAssert.AreEquivalent(new[] { _station }, new WorldDiff(_world, _world2).Stations.Added.Values);
        }

        [Test]
        public void Added_Ship()
        {
            _world2.AddShip(_ship);
            CollectionAssert.AreEquivalent(new[] { _ship }, new WorldDiff(_world, _world2).Ships.Added.Values);
        }

        [Test]
        public void Added_Inventory()
        {
            _world2.AddInventory(_inventory);
            CollectionAssert.AreEquivalent(new[] { _inventory }, new WorldDiff(_world, _world2).Inventories.Added.Values);
        }

        [Test]
        public void Removed_Planet()
        {
            _world.AddPlanet(_planet);
            CollectionAssert.AreEquivalent(new[] { _planet }, new WorldDiff(_world, _world2).Planets.Removed.Values);
        }

        [Test]
        public void Removed_Station()
        {
            _world.AddStation(_station);
            CollectionAssert.AreEquivalent(new[] { _station }, new WorldDiff(_world, _world2).Stations.Removed.Values);
        }

        [Test]
        public void Removed_Ship()
        {
            _world.AddShip(_ship);
            CollectionAssert.AreEquivalent(new[] { _ship }, new WorldDiff(_world, _world2).Ships.Removed.Values);
        }

        [Test]
        public void Removed_Inventory()
        {
            _world.AddInventory(_inventory);
            CollectionAssert.AreEquivalent(new[] { _inventory }, new WorldDiff(_world, _world2).Inventories.Removed.Values);
        }

        [Test]
        public void Modified_Planet()
        {
            var planet2 = new Planet(_planet.ID, "Jupiter");
            _world.AddPlanet(_planet);
            _world2.AddPlanet(planet2);
            var diff = new WorldDiff(_world, _world2);
            CollectionAssert.AreEquivalent(new[] { _planet }, diff.Planets.Removed.Values);
            CollectionAssert.AreEquivalent(new[] { planet2 }, diff.Planets.Added.Values);
        }
    }
}
