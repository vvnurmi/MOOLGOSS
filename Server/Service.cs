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
        private Dictionary<Guid, Station> _stations = new Dictionary<Guid, Station>();
        private Dictionary<Guid, Ship> _ships = new Dictionary<Guid, Ship>();
        private Dictionary<Guid, Inventory> _inventories = new Dictionary<Guid, Inventory>();

        public Planet[] GetPlanets()
        {
            return new[] { new Planet("Earth") };
        }

        public Station[] GetStations()
        {
            return _stations.Values.ToArray();
        }

        public Ship[] GetShips()
        {
            return _ships.Values.ToArray();
        }

        public Ship GetShip(Guid id)
        {
            Ship ship;
            _ships.TryGetValue(id, out ship);
            return ship;
        }

        public void AddStation(Guid id, Vector3 pos)
        {
            _stations[id] = new Station(id) { Pos = pos };
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
            if (!_ships.TryGetValue(id, out ship))
                _ships.Add(id, ship = new Ship(id, Vector3.Zero, Vector3.UnitX, Vector3.UnitY));
            return ship;
        }

        private Inventory GetOrAddInventory(Guid id)
        {
            Inventory inventory;
            if (!_inventories.TryGetValue(id, out inventory))
                _inventories.Add(id, inventory = new Inventory(id));
            return inventory;
        }
    }
}
