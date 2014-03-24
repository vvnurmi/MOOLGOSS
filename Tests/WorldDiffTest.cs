using Axiom.Math;
using Core;
using Core.Wobs;
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
            _ship = new Ship(Guid.NewGuid(), new Pose(new Vector3(5, 6, 7), Vector3.UnitX, Vector3.UnitY));
            _inventory = new Inventory(Guid.NewGuid());
        }

        [Test]
        public void EmptyForIdenticals()
        {
            var world2 = World.Empty.SetWob(_planet);
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
            var world = World.Empty.SetWob(_planet);
            var world2 = World.Empty.SetWob(planetClone);
            Assert.True(new WorldDiff(world, world2).IsEmpty);
        }

        [Test]
        public void Added_Wobs()
        {
            AssertDiffAndPatch(
                World.Empty.SetWob(_planet).SetWob(_station).SetWob(_ship).SetWob(_inventory),
                d => CollectionAssert.AreEquivalent(new Wob[] { _planet, _station, _ship, _inventory }, d.Wobs.Added.Values));
        }

        [Test]
        public void Removed_Wobs()
        {
            AssertDiffAndPatch(
                World.Empty.SetWob(_planet).SetWob(_station).SetWob(_ship).SetWob(_inventory),
                World.Empty,
                d => CollectionAssert.AreEquivalent(new Wob[] { _planet, _station, _ship, _inventory }, d.Wobs.Removed.Values));
        }

        [Test]
        public void Modified_Wobs()
        {
            var planet2 = new Planet(_planet.ID, "Jupiter");
            var ship2 = new Ship(_ship.ID, _ship.Pose.Move(new Vector3(50, 0, 0), 0, 0, 0));
            AssertDiffAndPatch(
                World.Empty.SetWob(_planet).SetWob(_ship),
                World.Empty.SetWob(planet2).SetWob(ship2),
                d =>
                {
                    CollectionAssert.AreEquivalent(new Wob[] { _planet, _ship }, d.Wobs.Removed.Values);
                    CollectionAssert.AreEquivalent(new Wob[] { planet2, ship2 }, d.Wobs.Added.Values);
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
