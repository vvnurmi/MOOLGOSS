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
        public void EmptyForIdenticals()
        {
            var world2 = World.Empty.SetPlanet(_planet);
            Assert.True(new WorldDiff(World.Empty, World.Empty).IsEmpty);
            Assert.True(new WorldDiff(world2, world2).IsEmpty);
            var diff = new WorldDiff(World.Empty, world2);
            Assert.False(diff.IsEmpty);
            Assertions.WorldsEqual(world2, World.Empty.Patch(diff));
        }

        [Test]
        public void ItemsMatchedByID()
        {
            var planetClone = new Planet(_planet.ID, _planet.Name);
            var world = World.Empty.SetPlanet(_planet);
            var world2 = World.Empty.SetPlanet(planetClone);
            Assert.True(new WorldDiff(world, world2).IsEmpty);
        }

        [Test]
        public void Added_Planet()
        {
            AssertDiffAndPatch(
                World.Empty.SetPlanet(_planet),
                d => CollectionAssert.AreEquivalent(new[] { _planet }, d.Planets.Added.Values));
        }

        [Test]
        public void Added_Station()
        {
            AssertDiffAndPatch(
                World.Empty.SetStation(_station),
                d => CollectionAssert.AreEquivalent(new[] { _station }, d.Stations.Added.Values));
        }

        [Test]
        public void Added_Ship()
        {
            AssertDiffAndPatch(
                World.Empty.SetShip(_ship),
                d => CollectionAssert.AreEquivalent(new[] { _ship }, d.Ships.Added.Values));
        }

        [Test]
        public void Added_Inventory()
        {
            AssertDiffAndPatch(
                World.Empty.SetInventory(_inventory),
                d => CollectionAssert.AreEquivalent(new[] { _inventory }, d.Inventories.Added.Values));
        }

        [Test]
        public void Removed_Planet()
        {
            AssertDiffAndPatch(
                World.Empty.SetPlanet(_planet),
                World.Empty,
                d => CollectionAssert.AreEquivalent(new[] { _planet }, d.Planets.Removed.Values));
        }

        [Test]
        public void Removed_Station()
        {
            AssertDiffAndPatch(
                World.Empty.SetStation(_station),
                World.Empty,
                d => CollectionAssert.AreEquivalent(new[] { _station }, d.Stations.Removed.Values));
        }

        [Test]
        public void Removed_Ship()
        {
            AssertDiffAndPatch(
                World.Empty.SetShip(_ship),
                World.Empty,
                d => CollectionAssert.AreEquivalent(new[] { _ship }, d.Ships.Removed.Values));
        }

        [Test]
        public void Removed_Inventory()
        {
            AssertDiffAndPatch(
                World.Empty.SetInventory(_inventory),
                World.Empty,
                d => CollectionAssert.AreEquivalent(new[] { _inventory }, d.Inventories.Removed.Values));
        }

        [Test]
        public void Modified_Planet()
        {
            var planet2 = new Planet(_planet.ID, "Jupiter");
            AssertDiffAndPatch(
                World.Empty.SetPlanet(_planet),
                World.Empty.SetPlanet(planet2),
                d =>
                {
                    CollectionAssert.AreEquivalent(new[] { _planet }, d.Planets.Removed.Values);
                    CollectionAssert.AreEquivalent(new[] { planet2 }, d.Planets.Added.Values);
                });
        }

        [Test]
        public void Modified_Ship()
        {
            var ship2 = new Ship(_ship.ID, _ship.Pos + new Vector3(50, 0, 0), _ship.Front, _ship.Up);
            AssertDiffAndPatch(
                World.Empty.SetShip(_ship),
                World.Empty.SetShip(ship2),
                d =>
                {
                    CollectionAssert.AreEquivalent(new[] { _ship }, d.Ships.Removed.Values);
                    CollectionAssert.AreEquivalent(new[] { ship2 }, d.Ships.Added.Values);
                });
        }

        private void AssertDiffAndPatch(World world2, Action<WorldDiff> assertDiff)
        {
            AssertDiffAndPatch(World.Empty, world2, assertDiff);
        }

        private void AssertDiffAndPatch(World world1, World world2, Action<WorldDiff> assertDiff)
        {
            var diff = new WorldDiff(world1, world2);
            assertDiff(diff);
            Assertions.WorldsEqual(world2, world1.Patch(diff));
        }
    }
}
