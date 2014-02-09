using Core.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class World
    {
        private Dictionary<Guid, Station> _stations = new Dictionary<Guid, Station>();
        private Dictionary<Guid, Ship> _ships = new Dictionary<Guid, Ship>();
        private Dictionary<Guid, Inventory> _inventories = new Dictionary<Guid, Inventory>();

        public Dictionary<Guid, Station> Stations { get { return _stations; } }
        public Dictionary<Guid, Ship> Ships { get { return _ships; } }
        public Dictionary<Guid, Inventory> Inventories { get { return _inventories; } }
    }
}
