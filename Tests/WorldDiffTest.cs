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
            _station = new Station(Guid.NewGuid(), new Vector3(10, 0, 20));
            _ship = new Ship(Guid.NewGuid(), new Vector3(5, 6, 7), Vector3.UnitX, Vector3.UnitY);
            _inventory = new Inventory(Guid.NewGuid());
        }

        [Test]
        public void PatchReturnsSelf()
        {
            Assert.AreSame(_world, _world.Patch(new WorldDiff(_world, _world)));
        }

        [Test]
        public void EmptyForIdenticals()
        {
            _world2.SetPlanet(_planet);
            Assert.True(new WorldDiff(_world, _world).IsEmpty);
            Assert.True(new WorldDiff(_world2, _world2).IsEmpty);
            var diff = new WorldDiff(_world, _world2);
            Assert.False(diff.IsEmpty);
            Assertions.WorldsEqual(_world2, _world.Patch(diff));
        }

        [Test]
        public void ItemsMatchedByID()
        {
            var planetClone = new Planet(_planet.ID, _planet.Name);
            _world.SetPlanet(_planet);
            _world2.SetPlanet(planetClone);
            Assert.True(new WorldDiff(_world, _world2).IsEmpty);
        }

        [Test]
        public void Added_Planet()
        {
            AssertDiffAndPatch(
                w => w.SetPlanet(_planet),
                d => CollectionAssert.AreEquivalent(new[] { _planet }, d.Planets.Added.Values));
        }

        [Test]
        public void Added_Station()
        {
            AssertDiffAndPatch(
                w => w.SetStation(_station),
                d => CollectionAssert.AreEquivalent(new[] { _station }, d.Stations.Added.Values));
        }

        [Test]
        public void Added_Ship()
        {
            AssertDiffAndPatch(
                w => w.SetShip(_ship),
                d => CollectionAssert.AreEquivalent(new[] { _ship }, d.Ships.Added.Values));
        }

        [Test]
        public void Added_Inventory()
        {
            AssertDiffAndPatch(
                w => w.SetInventory(_inventory),
                d => CollectionAssert.AreEquivalent(new[] { _inventory }, d.Inventories.Added.Values));
        }

        [Test]
        public void Removed_Planet()
        {
            AssertDiffAndPatch(
                w => w.SetPlanet(_planet),
                w2 => { },
                d => CollectionAssert.AreEquivalent(new[] { _planet }, d.Planets.Removed.Values));
        }

        [Test]
        public void Removed_Station()
        {
            AssertDiffAndPatch(
                w => w.SetStation(_station),
                w2 => { },
                d => CollectionAssert.AreEquivalent(new[] { _station }, d.Stations.Removed.Values));
        }

        [Test]
        public void Removed_Ship()
        {
            AssertDiffAndPatch(
                w => w.SetShip(_ship),
                w2 => { },
                d => CollectionAssert.AreEquivalent(new[] { _ship }, d.Ships.Removed.Values));
        }

        [Test]
        public void Removed_Inventory()
        {
            AssertDiffAndPatch(
                w => w.SetInventory(_inventory),
                w2 => { },
                d => CollectionAssert.AreEquivalent(new[] { _inventory }, d.Inventories.Removed.Values));
        }

        [Test]
        public void Modified_Planet()
        {
            var planet2 = new Planet(_planet.ID, "Jupiter");
            AssertDiffAndPatch(
                w => w.SetPlanet(_planet),
                w2 => w2.SetPlanet(planet2),
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
                w => w.SetShip(_ship),
                w2 => w2.SetShip(ship2),
                d =>
                {
                    CollectionAssert.AreEquivalent(new[] { _ship }, d.Ships.Removed.Values);
                    CollectionAssert.AreEquivalent(new[] { ship2 }, d.Ships.Added.Values);
                });
        }

        private void AssertDiffAndPatch(Action<World> buildWorld, Action<WorldDiff> assertDiff)
        {
            AssertDiffAndPatch(w => { }, buildWorld, assertDiff);
        }

        private void AssertDiffAndPatch(Action<World> buildWorld1, Action<World> buildWorld2, Action<WorldDiff> assertDiff)
        {
            buildWorld1(_world);
            buildWorld2(_world2);
            var diff = new WorldDiff(_world, _world2);
            assertDiff(diff);
            Assertions.WorldsEqual(_world2, _world.Patch(diff));
        }
    }
}
