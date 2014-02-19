using Core.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Diff<T> where T : IEquatable<T>
    {
        private class Comparer : IEqualityComparer<KeyValuePair<Guid, T>>
        {
            public bool Equals(KeyValuePair<Guid, T> x, KeyValuePair<Guid, T> y)
            {
                return x.Key == y.Key && x.Value.Equals(y.Value);
            }

            public int GetHashCode(KeyValuePair<Guid, T> obj) { return obj.GetHashCode(); }
        }
        private static Comparer g_comparer = new Comparer();

        private readonly Dictionary<Guid, T> _removed;
        private readonly Dictionary<Guid, T> _added;

        public bool IsEmpty { get { return !_removed.Any() && !_added.Any(); } }
        public IReadOnlyDictionary<Guid, T> Removed { get { return _removed; } }
        public IReadOnlyDictionary<Guid, T> Added { get { return _added; } }

        public Diff(IReadOnlyDictionary<Guid, T> before, IReadOnlyDictionary<Guid, T> after)
        {
            _removed = before.Except(after, g_comparer).ToDictionary(x => x.Key, x => x.Value);
            _added = after.Except(before, g_comparer).ToDictionary(x => x.Key, x => x.Value);
        }
    }

    public class WorldDiff
    {
        public bool IsEmpty { get { return Planets.IsEmpty && Stations.IsEmpty && Ships.IsEmpty && Inventories.IsEmpty; } }
        public Diff<Planet> Planets { get; private set; }
        public Diff<Station> Stations { get; private set; }
        public Diff<Ship> Ships { get; private set; }
        public Diff<Inventory> Inventories { get; private set; }

        public WorldDiff(World before, World after)
        {
            Planets = new Diff<Planet>(before.Planets, after.Planets);
            Stations = new Diff<Station>(before.Stations, after.Stations);
            Ships = new Diff<Ship>(before.Ships, after.Ships);
            Inventories = new Diff<Inventory>(before.Inventories, after.Inventories);
        }
    }
}
