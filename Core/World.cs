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
        private readonly ImmutableDictionary<Guid, Wob> _wobs;
        private readonly ImmutableDictionary<Guid, Guid> _playerShipIDs;

        public static readonly World Empty = new World();

        public IReadOnlyDictionary<Guid, Wob> Wobs { get { return _wobs; } }
        public IReadOnlyDictionary<Guid, Guid> PlayerShipIDs { get { return _playerShipIDs; } }

        private World()
        {
            _wobs = ImmutableDictionary<Guid, Wob>.Empty;
            _playerShipIDs = ImmutableDictionary<Guid, Guid>.Empty;
        }

        private World(ImmutableDictionary<Guid, Wob> wobs, ImmutableDictionary<Guid, Guid> playerShipIDs)
        {
            _wobs = wobs;
            _playerShipIDs = playerShipIDs;
        }

        public T GetWob<T>(Guid id) where T : Wob
        {
            Wob wob;
            if (!_wobs.TryGetValue(id, out wob)) return null;
            return (T)wob;
        }

        public Guid GetPlayerShipID(Guid playerID)
        {
            return _playerShipIDs[playerID];
        }

        public World SetWob(Wob wob)
        {
            if (wob == null) throw new ArgumentNullException();
            return new World(_wobs.SetItem(wob.ID, wob), _playerShipIDs);
        }

        public World SetPlayerShipID(Guid playerID, Guid shipID)
        {
            return new World(_wobs, _playerShipIDs.Add(playerID, shipID));
        }

        public World RemoveWob(Guid id)
        {
            return new World(_wobs.Remove(id), _playerShipIDs);
        }

        public World RemovePlayerShipID(Guid playerID)
        {
            return new World(_wobs, _playerShipIDs.Remove(playerID));
        }

        public World Patch(WorldDiff diff)
        {
            var wobs = _wobs.RemoveRange(diff.Wobs.Removed.Keys).SetItems(diff.Wobs.Added);
            return new World(wobs, _playerShipIDs);
        }
    }
}
