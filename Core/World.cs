﻿using Core.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class World
    {
        private Dictionary<Guid, Planet> _planets = new Dictionary<Guid, Planet>();
        private Dictionary<Guid, Station> _stations = new Dictionary<Guid, Station>();
        private Dictionary<Guid, Ship> _ships = new Dictionary<Guid, Ship>();
        private Dictionary<Guid, Inventory> _inventories = new Dictionary<Guid, Inventory>();

        public IEnumerable<Planet> Planets { get { return _planets.Values; } }
        public IEnumerable<Station> Stations { get { return _stations.Values; } }
        public IEnumerable<Ship> Ships { get { return _ships.Values; } }
        public IEnumerable<Inventory> Inventories { get { return _inventories.Values; } }

        public Planet GetPlanet(Guid id)
        {
            Planet planet;
            if (!_planets.TryGetValue(id, out planet)) return null;
            return planet;
        }

        public Station GetStation(Guid id)
        {
            Station station;
            if (!_stations.TryGetValue(id, out station)) return null;
            return station;
        }

        public Ship GetShip(Guid id)
        {
            Ship ship;
            if (!_ships.TryGetValue(id, out ship)) return null;
            return ship;
        }

        public Inventory GetInventory(Guid id)
        {
            Inventory inventory;
            if (!_inventories.TryGetValue(id, out inventory)) return null;
            return inventory;
        }

        public void AddPlanet(Planet planet)
        {
            if (planet == null) throw new ArgumentNullException();
            _planets.Add(planet.ID, planet);
        }

        public void AddStation(Station station)
        {
            if (station == null) throw new ArgumentNullException();
            _stations.Add(station.ID, station);
        }

        public void AddShip(Ship ship)
        {
            if (ship == null) throw new ArgumentNullException();
            _ships.Add(ship.ID, ship);
        }

        public void AddInventory(Inventory inventory)
        {
            if (inventory == null) throw new ArgumentNullException();
            _inventories.Add(inventory.ID, inventory);
        }

        public bool RemovePlanet(Guid id)
        {
            return _planets.Remove(id);
        }

        public bool RemoveStation(Guid id)
        {
            return _stations.Remove(id);
        }

        public bool RemoveShip(Guid id)
        {
            return _ships.Remove(id);
        }

        public bool RemoveInventory(Guid id)
        {
            return _inventories.Remove(id);
        }
    }
}
