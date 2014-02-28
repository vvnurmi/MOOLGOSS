using Core.Items;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class World
    {
        private readonly ImmutableDictionary<Guid, Planet> _planets;
        private readonly ImmutableDictionary<Guid, Station> _stations;
        private readonly ImmutableDictionary<Guid, Ship> _ships;
        private readonly ImmutableDictionary<Guid, Inventory> _inventories;

        public static readonly World Empty = new World();

        public IReadOnlyDictionary<Guid, Planet> Planets { get { return _planets; } }
        public IReadOnlyDictionary<Guid, Station> Stations { get { return _stations; } }
        public IReadOnlyDictionary<Guid, Ship> Ships { get { return _ships; } }
        public IReadOnlyDictionary<Guid, Inventory> Inventories { get { return _inventories; } }

        private World()
        {
            _planets = ImmutableDictionary<Guid, Planet>.Empty;
            _stations = ImmutableDictionary<Guid, Station>.Empty;
            _ships = ImmutableDictionary<Guid, Ship>.Empty;
            _inventories = ImmutableDictionary<Guid, Inventory>.Empty;
        }

        private World(ImmutableDictionary<Guid, Planet> planets, ImmutableDictionary<Guid, Station> stations,
            ImmutableDictionary<Guid, Ship> ships, ImmutableDictionary<Guid, Inventory> inventories)
        {
            _planets = planets;
            _stations = stations;
            _ships = ships;
            _inventories = inventories;
        }

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

        public World SetPlanet(Planet planet)
        {
            if (planet == null) throw new ArgumentNullException();
            return new World(_planets.SetItem(planet.ID, planet), _stations, _ships, _inventories);
        }

        public World SetStation(Station station)
        {
            if (station == null) throw new ArgumentNullException();
            return new World(_planets, _stations.SetItem(station.ID, station), _ships, _inventories);
        }

        public World SetShip(Ship ship)
        {
            if (ship == null) throw new ArgumentNullException();
            return new World(_planets, _stations, _ships.SetItem(ship.ID, ship), _inventories);
        }

        public World SetInventory(Inventory inventory)
        {
            if (inventory == null) throw new ArgumentNullException();
            return new World(_planets, _stations, _ships, _inventories.SetItem(inventory.ID, inventory));
        }

        public World RemovePlanet(Guid id)
        {
            return new World(_planets.Remove(id), _stations, _ships, _inventories);
        }

        public World RemoveStation(Guid id)
        {
            return new World(_planets, _stations.Remove(id), _ships, _inventories);
        }

        public World RemoveShip(Guid id)
        {
            return new World(_planets, _stations, _ships.Remove(id), _inventories);
        }

        public World RemoveInventory(Guid id)
        {
            return new World(_planets, _stations, _ships, _inventories.Remove(id));
        }

        /// <summary>
        /// Applies a patch to the world. Returns self.
        /// </summary>
        public World Patch(WorldDiff diff)
        {
            var planets = _planets.RemoveRange(diff.Planets.Removed.Keys).SetItems(diff.Planets.Added);
            var stations = _stations.RemoveRange(diff.Stations.Removed.Keys).SetItems(diff.Stations.Added);
            var ships = _ships.RemoveRange(diff.Ships.Removed.Keys).SetItems(diff.Ships.Added);
            var inventories = _inventories.RemoveRange(diff.Inventories.Removed.Keys).SetItems(diff.Inventories.Added);
            return new World(planets, stations, ships, inventories);
        }
    }
}
