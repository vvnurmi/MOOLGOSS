using Core.Items;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public sealed class World
    {
        private readonly ImmutableDictionary<Guid, Wob> _wobs;
        private readonly ImmutableDictionary<Guid, Guid> _playerShipIDs;
        private readonly ImmutableHashSet<Guid> _watchWobIDs;

        public static readonly World Empty = new World();

        public IReadOnlyDictionary<Guid, Wob> Wobs { get { return _wobs; } }
        public IReadOnlyDictionary<Guid, Guid> PlayerShipIDs { get { return _playerShipIDs; } }

        private World()
        {
            _wobs = ImmutableDictionary<Guid, Wob>.Empty;
            _playerShipIDs = ImmutableDictionary<Guid, Guid>.Empty;
            _watchWobIDs = ImmutableHashSet<Guid>.Empty;
        }

        private World(ImmutableDictionary<Guid, Wob> wobs, ImmutableDictionary<Guid, Guid> playerShipIDs,
            ImmutableHashSet<Guid> watchWobIDs)
        {
            _wobs = wobs;
            _playerShipIDs = playerShipIDs;
            _watchWobIDs = watchWobIDs;
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
            if (wob == GetWob<Wob>(wob.ID)) return this;
            return SetWobs(_wobs.SetItem(wob.ID, wob))
                .WobWatchCheck(wob.ID);
        }

        public World SetPlayerShipID(Guid playerID, Guid shipID)
        {
            return SetPlayerShipIDs(_playerShipIDs.Add(playerID, shipID));
        }

        public World AddWobWatch(Guid id)
        {
            return SetWatchWobIDs(_watchWobIDs.Add(id));
        }

        public World RemoveWob(Guid id)
        {
            return SetWobs(_wobs.Remove(id))
                .WobWatchCheck(id);
        }

        public World RemovePlayerShipID(Guid playerID)
        {
            return SetPlayerShipIDs(_playerShipIDs.Remove(playerID));
        }

        public World RemoveWobWatch(Guid id)
        {
            return SetWatchWobIDs(_watchWobIDs.Remove(id));
        }

        public World Patch(WorldDiff diff)
        {
            return SetWobs(_wobs.RemoveRange(diff.Wobs.Removed.Keys).SetItems(diff.Wobs.Added))
                .WobWatchCheck(diff.Wobs.Added.Keys.Union(diff.Wobs.Removed.Keys));
        }

        private World SetWobs(ImmutableDictionary<Guid, Wob> wobs) { return new World(wobs, _playerShipIDs, _watchWobIDs); }
        private World SetPlayerShipIDs(ImmutableDictionary<Guid, Guid> ids) { return new World(_wobs, ids, _watchWobIDs); }
        private World SetWatchWobIDs(ImmutableHashSet<Guid> ids) { return new World(_wobs, _playerShipIDs, ids); }

        private World WobWatchCheck(Guid id)
        {
            if (_watchWobIDs.Contains(id)) WriteWobWatchTrace(id);
            return this;
        }

        private World WobWatchCheck(IEnumerable<Guid> ids)
        {
            foreach (var id in _watchWobIDs.Intersect(ids)) WriteWobWatchTrace(id);
            return this;
        }

        private void WriteWobWatchTrace(Guid id)
        {
            Trace.WriteLine(string.Format("Wob {0} changed to {1}\n{2}",
                id, (object)GetWob<Wob>(id) ?? "<null>", new StackTrace(2, fNeedFileInfo: true)));
        }
    }
}
