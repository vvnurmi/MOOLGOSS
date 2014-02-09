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
        private World _world = new World();

        public Planet[] GetPlanets()
        {
            return new[] { new Planet("Earth") };
        }

        public Station[] GetStations()
        {
            return _world.Stations.Values.ToArray();
        }

        public Ship[] GetShips()
        {
            return _world.Ships.Values.ToArray();
        }

        public Ship GetShip(Guid id)
        {
            Ship ship;
            _world.Ships.TryGetValue(id, out ship);
            return ship;
        }

        public void AddStation(Guid id, Vector3 pos)
        {
            _world.Stations[id] = new Station(id) { Pos = pos };
        }

        public void UpdateShip(Guid id, Vector3 pos, Vector3 front, Vector3 up)
        {
            var ship = GetOrAddShip(id);
            ship.Set(pos, front, up);
        }

        public Inventory GetInventory(Guid id)
        {
            return GetOrAddInventory(id);
        }

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

        private Ship GetOrAddShip(Guid id)
        {
            Ship ship;
            if (!_world.Ships.TryGetValue(id, out ship))
                _world.Ships.Add(id, ship = new Ship(id, Vector3.Zero, Vector3.UnitX, Vector3.UnitY));
            return ship;
        }

        private Inventory GetOrAddInventory(Guid id)
        {
            Inventory inventory;
            if (!_world.Inventories.TryGetValue(id, out inventory))
                _world.Inventories.Add(id, inventory = new Inventory(id));
            return inventory;
        }
    }
}
