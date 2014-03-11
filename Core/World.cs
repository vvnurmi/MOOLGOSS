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

        public static readonly World Empty = new World();

        public IReadOnlyDictionary<Guid, Wob> Wobs { get { return _wobs; } }

        private World()
        {
            _wobs = ImmutableDictionary<Guid, Wob>.Empty;
        }

        private World(ImmutableDictionary<Guid, Wob> wobs)
        {
            _wobs = wobs;
        }

        public T GetWob<T>(Guid id) where T : Wob
        {
            Wob wob;
            if (!_wobs.TryGetValue(id, out wob)) return null;
            return (T)wob;
        }

        public World SetWob(Wob wob)
        {
            if (wob == null) throw new ArgumentNullException();
            return new World(_wobs.SetItem(wob.ID, wob));
        }

        public World RemoveWob(Guid id)
        {
            return new World(_wobs.Remove(id));
        }

        /// <summary>
        /// Applies a patch to the world. Returns self.
        /// </summary>
        public World Patch(WorldDiff diff)
        {
            var wobs = _wobs.RemoveRange(diff.Wobs.Removed.Keys).SetItems(diff.Wobs.Added);
            return new World(wobs);
        }
    }
}
