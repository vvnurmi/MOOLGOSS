using Axiom.Math;
using Core;
using Core.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Service : IService
    {
        private World _world;
        private Dictionary<Guid, World> _worldShadows = new Dictionary<Guid, World>();

        public Service(World world)
        {
            _world = world;
        }

        public void SendWorldPatch(Guid clientID, WorldDiff diff)
        {
            GetShadow(clientID).Patch(diff);
            _world.Patch(diff);
        }

        public WorldDiff ReceiveWorldPatch(Guid clientID)
        {
            var shadow = GetShadow(clientID);
            var diff = new WorldDiff(shadow, _world);
            shadow.Patch(diff);
            return diff;
        }

        private World GetShadow(Guid clientID)
        {
            World shadow;
            if (!_worldShadows.TryGetValue(clientID, out shadow))
                _worldShadows.Add(clientID, shadow = new World());
            return shadow;
        }

        [Obsolete]
        public Service()
        {
            _world = new World();
            _world.AddPlanet(new Planet(Guid.NewGuid(), "Earth"));
        }

        [Obsolete]
        public Planet[] GetPlanets()
        {
            return _world.Planets.Values.ToArray();
        }

        [Obsolete]
        public Station[] GetStations()
        {
            return _world.Stations.Values.ToArray();
        }

        [Obsolete]
        public Ship[] GetShips()
        {
            return _world.Ships.Values.ToArray();
        }

        [Obsolete]
        public Ship GetShip(Guid id)
        {
            return _world.GetShip(id);
        }

        [Obsolete]
        public void AddStation(Guid id, Vector3 pos)
        {
            try { _world.RemoveStation(id); }
            catch (ArgumentException) { }
            _world.AddStation(new Station(id) { Pos = pos });
        }

        [Obsolete]
        public void UpdateShip(Guid id, Vector3 pos, Vector3 front, Vector3 up)
        {
            var ship = GetOrAddShip(id);
            ship.Set(pos, front, up);
        }

        [Obsolete]
        public Inventory GetInventory(Guid id)
        {
            return GetOrAddInventory(id);
        }

        [Obsolete]
        public void AddToInventory(Guid id, ItemStack stack)
        {
            var inventory = GetOrAddInventory(id);
            try
            {
                inventory.Add(stack);
            }
            catch (InvalidOperationException)
            {
            }
        }

        [Obsolete]
        private Ship GetOrAddShip(Guid id)
        {
            var ship = _world.GetShip(id);
            if (ship == null) _world.AddShip(ship = new Ship(id, Vector3.Zero, Vector3.UnitX, Vector3.UnitY));
            return ship;
        }

        [Obsolete]
        private Inventory GetOrAddInventory(Guid id)
        {
            var inventory = _world.GetInventory(id);
            if (inventory == null) _world.AddInventory(inventory = new Inventory(id));
            return inventory;
        }
    }
}
