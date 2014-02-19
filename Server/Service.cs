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

        public Service()
        {
            _world.AddPlanet(new Planet(Guid.NewGuid(), "Earth"));
        }

        public Planet[] GetPlanets()
        {
            return _world.Planets.Values.ToArray();
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
            return _world.GetShip(id);
        }

        public void AddStation(Guid id, Vector3 pos)
        {
            try { _world.RemoveStation(id); }
            catch (ArgumentException) { }
            _world.AddStation(new Station(id) { Pos = pos });
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
            var ship = _world.GetShip(id);
            if (ship == null) _world.AddShip(ship = new Ship(id, Vector3.Zero, Vector3.UnitX, Vector3.UnitY));
            return ship;
        }

        private Inventory GetOrAddInventory(Guid id)
        {
            var inventory = _world.GetInventory(id);
            if (inventory == null) _world.AddInventory(inventory = new Inventory(id));
            return inventory;
        }
    }
}
