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
    public class WorldTest
    {
        private Planet _planet;
        private Station _station;
        private Ship _ship;
        private Inventory _inventory;
        private Guid _playerID;
        private Guid _shipID;

        [SetUp]
        public void Setup()
        {
            _planet = new Planet(Guid.NewGuid(), "Earth");
            _station = new Station(Guid.NewGuid(), new Vector3(10, 0, 20));
            _ship = new Ship(Guid.NewGuid(), new Vector3(5, 6, 7), Vector3.UnitX, Vector3.UnitY);
            _inventory = new Inventory(Guid.NewGuid());
            _playerID = Guid.NewGuid();
            _shipID = Guid.NewGuid();
        }

        [Test]
        public void AfterCreation_IsEmpty()
        {
            CollectionAssert.IsEmpty(World.Empty.Wobs);
        }

        [Test]
        public void AfterCreation_NoPlayerShips()
        {
            CollectionAssert.IsEmpty(World.Empty.PlayerShipIDs);
        }

        [Test]
        public void SetItems()
        {
            var world = GetWorldWithItems();
            CollectionAssert.AreEquivalent(new Wob[] { _planet, _station, _ship, _inventory }, world.Wobs.Values);
        }

        [Test]
        public void SetNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => World.Empty.SetWob(null));
        }

        [Test]
        public void RemovingNonexistent_DoesntThrow()
        {
            World.Empty.RemoveWob(_planet.ID);
            World.Empty.RemoveWob(_station.ID);
            World.Empty.RemoveWob(_ship.ID);
            World.Empty.RemoveWob(_inventory.ID);
        }

        [Test]
        public void RemoveItems()
        {
            var world = GetWorldWithItems();
            world = world.RemoveWob(_planet.ID)
                .RemoveWob(_station.ID)
                .RemoveWob(_ship.ID)
                .RemoveWob(_inventory.ID);
            CollectionAssert.IsEmpty(world.Wobs);
        }

        [Test]
        public void GetItems()
        {
            Assert.IsNull(World.Empty.GetWob<Planet>(_planet.ID));
            Assert.IsNull(World.Empty.GetWob<Station>(_station.ID));
            Assert.IsNull(World.Empty.GetWob<Ship>(_ship.ID));
            Assert.IsNull(World.Empty.GetWob<Inventory>(_inventory.ID));
            var world = GetWorldWithItems();
            Assert.AreEqual(_planet, world.GetWob<Planet>(_planet.ID));
            Assert.AreEqual(_station, world.GetWob<Station>(_station.ID));
            Assert.AreEqual(_ship, world.GetWob<Ship>(_ship.ID));
            Assert.AreEqual(_inventory, world.GetWob<Inventory>(_inventory.ID));
        }

        [Test]
        public void GetPlayerShipID_InvalidThrows()
        {
            Assert.Throws<KeyNotFoundException>(() => World.Empty.GetPlayerShipID(Guid.NewGuid()));
        }

        [Test]
        public void GetPlayerShipID()
        {
            var world = World.Empty.SetPlayerShipID(_playerID, _shipID);
            Assert.AreEqual(_shipID, world.GetPlayerShipID(_playerID));
        }

        [Test]
        public void SetPlayerShipID_TwiceThrows()
        {
            var world = World.Empty.SetPlayerShipID(_playerID, _shipID);
            Assert.Throws<ArgumentException>(() => world.SetPlayerShipID(_playerID, Guid.NewGuid()));
        }

        [Test]
        public void RemovePlayerShipID_NonExistent_DoesntThrow()
        {
            World.Empty.RemovePlayerShipID(Guid.NewGuid());
        }

        [Test]
        public void RemovePlayerShipID()
        {
            var world = World.Empty
                .SetPlayerShipID(_playerID, _shipID)
                .RemovePlayerShipID(_playerID);
            Assert.Throws<KeyNotFoundException>(() => world.GetPlayerShipID(_playerID));
        }

        private World GetWorldWithItems()
        {
            return World.Empty
                .SetWob(_planet)
                .SetWob(_station)
                .SetWob(_ship)
                .SetWob(_inventory);
        }
    }
}
