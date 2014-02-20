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
        public void PatchReturnsSelf()
        {
            Assert.AreSame(_world, _world.Patch(new WorldDiff(_world, _world)));
        }

        [Test]
        public void EmptyForIdenticals()
        {
            _world2.AddPlanet(_planet);
            Assert.True(new WorldDiff(_world, _world).IsEmpty);
            Assert.True(new WorldDiff(_world2, _world2).IsEmpty);
            var diff = new WorldDiff(_world, _world2);
            Assert.False(diff.IsEmpty);
            AssertWorldsEqual(_world2, _world.Patch(diff));
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
            AssertDiffAndPatch(
                w => w.AddPlanet(_planet),
                d => CollectionAssert.AreEquivalent(new[] { _planet }, d.Planets.Added.Values));
        }

        [Test]
        public void Added_Station()
        {
            AssertDiffAndPatch(
                w => w.AddStation(_station),
                d => CollectionAssert.AreEquivalent(new[] { _station }, d.Stations.Added.Values));
        }

        [Test]
        public void Added_Ship()
        {
            AssertDiffAndPatch(
                w => w.AddShip(_ship),
                d => CollectionAssert.AreEquivalent(new[] { _ship }, d.Ships.Added.Values));
        }

        [Test]
        public void Added_Inventory()
        {
            AssertDiffAndPatch(
                w => w.AddInventory(_inventory),
                d => CollectionAssert.AreEquivalent(new[] { _inventory }, d.Inventories.Added.Values));
        }

        [Test]
        public void Removed_Planet()
        {
            AssertDiffAndPatch(
                w => w.AddPlanet(_planet),
                w2 => { },
                d => CollectionAssert.AreEquivalent(new[] { _planet }, d.Planets.Removed.Values));
        }

        [Test]
        public void Removed_Station()
        {
            AssertDiffAndPatch(
                w => w.AddStation(_station),
                w2 => { },
                d => CollectionAssert.AreEquivalent(new[] { _station }, d.Stations.Removed.Values));
        }

        [Test]
        public void Removed_Ship()
        {
            AssertDiffAndPatch(
                w => w.AddShip(_ship),
                w2 => { },
                d => CollectionAssert.AreEquivalent(new[] { _ship }, d.Ships.Removed.Values));
        }

        [Test]
        public void Removed_Inventory()
        {
            AssertDiffAndPatch(
                w => w.AddInventory(_inventory),
                w2 => { },
                d => CollectionAssert.AreEquivalent(new[] { _inventory }, d.Inventories.Removed.Values));
        }

        [Test]
        public void Modified_Planet()
        {
            var planet2 = new Planet(_planet.ID, "Jupiter");
            AssertDiffAndPatch(
                w => w.AddPlanet(_planet),
                w2 => w2.AddPlanet(planet2),
                d => {
                    CollectionAssert.AreEquivalent(new[] { _planet }, d.Planets.Removed.Values);
                    CollectionAssert.AreEquivalent(new[] { planet2 }, d.Planets.Added.Values);
                });
        }

        private void AppendSeqDiff<T>(StringBuilder str, string name, Diff<T> seqDiff) where T : IEquatable<T>
        {
            str.AppendFormat("{0} +{1} -{2}", name, seqDiff.Added.Count, seqDiff.Removed.Count);
        }

        private void AssertWorldsEqual(World expected, World actual)
        {
            var diff = new WorldDiff(expected, actual);
            var message = new StringBuilder("Worlds differ");
            AppendSeqDiff(message, ", Planets", diff.Planets);
            AppendSeqDiff(message, ", Stations", diff.Stations);
            AppendSeqDiff(message, ", Ships", diff.Ships);
            AppendSeqDiff(message, ", Inventories", diff.Inventories);
            Assert.True(diff.IsEmpty, message.ToString());
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
            AssertWorldsEqual(_world2, _world.Patch(diff));
        }
    }
}
