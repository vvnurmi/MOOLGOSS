using Axiom.Math;
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Service : IService
    {
        private Dictionary<Guid, Ship> _ships = new Dictionary<Guid, Ship>();

        public Planet[] GetPlanets()
        {
            return new[] { new Planet("Earth") };
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

        public void UpdateShip(Guid id, Vector3 pos, Vector3 front, Vector3 up)
        {
            var ship = GetOrAddShip(id);
            ship.Set(pos, front, up);
        }

        private Ship GetOrAddShip(Guid id)
        {
            Ship ship;
            if (!_ships.TryGetValue(id, out ship))
                _ships.Add(id, ship = new Ship(id, Vector3.Zero, Vector3.UnitX, Vector3.UnitY));
            return ship;
        }
    }
}
