using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Immutable
{
    public class ImmutableHashSet<T> : IImmutableSet<T>, ISet<T>, ICollection where T : IComparable<T>
    {
        public static readonly ImmutableHashSet<T> Empty = new ImmutableHashSet<T>(ImmutableDictionary<T, T>.Empty);

        private readonly ImmutableDictionary<T, T> _dic;

        public int Count { get { return _dic.Count; } }
        public bool IsReadOnly { get { return true; } }

        private ImmutableHashSet(ImmutableDictionary<T, T> dic) { _dic = dic; }

        private ImmutableHashSet<T> SetDic(ImmutableDictionary<T, T> dic) { return new ImmutableHashSet<T>(dic); }
        private ImmutableHashSet<T> SetDic(IEnumerable<T> keys)
        {
            return new ImmutableHashSet<T>(ImmutableDictionary<T, T>.Empty
                .AddRange(keys.Select(x => new KeyValuePair<T, T>(x, x))));
        }

        public ImmutableHashSet<T> Add(T value) { return SetDic(_dic.Add(value, value)); }

        public ImmutableHashSet<T> Clear() { return SetDic(_dic.Clear()); }

        public bool Contains(T value) { return _dic.ContainsKey(value); }

        public ImmutableHashSet<T> Except(IEnumerable<T> other) { return SetDic(_dic.Keys.Except(other)); }

        public ImmutableHashSet<T> Intersect(IEnumerable<T> other) { return SetDic(_dic.Keys.Intersect(other)); }

        public bool IsProperSubsetOf(IEnumerable<T> other) { return IsSubsetOf(other) && other.Except(_dic.Keys).Any(); }

        public bool IsProperSupersetOf(IEnumerable<T> other) { return IsSupersetOf(other) && _dic.Keys.Except(other).Any(); }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            var otherDic = other.ToDictionary(x => x); return this.All(otherDic.ContainsKey);
        }

        public bool IsSupersetOf(IEnumerable<T> other) { return other.All(Contains); }

        public bool Overlaps(IEnumerable<T> other) { return other.Any(_dic.ContainsKey); }

        public ImmutableHashSet<T> Remove(T value) { return SetDic(_dic.Remove(value)); }

        public bool SetEquals(IEnumerable<T> other) { return IsSubsetOf(other) && IsSupersetOf(other); }

        public ImmutableHashSet<T> SymmetricExcept(IEnumerable<T> other)
        {
            return SetDic(_dic.Keys.Except(other).Union(other.Except(_dic.Keys)));
        }

        public ImmutableHashSet<T> Union(IEnumerable<T> other)
        {
            return SetDic(_dic.Keys.Union(other));
        }

        public IEnumerator<T> GetEnumerator() { return _dic.Keys.GetEnumerator(); }

        public void CopyTo(T[] array, int arrayIndex) { _dic.Keys.ToArray().CopyTo(array, arrayIndex); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        IImmutableSet<T> IImmutableSet<T>.Add(T value) { return Add(value); }
        IImmutableSet<T> IImmutableSet<T>.Clear() { return Clear(); }
        IImmutableSet<T> IImmutableSet<T>.Except(IEnumerable<T> other) { return Except(other); }
        IImmutableSet<T> IImmutableSet<T>.Intersect(IEnumerable<T> other) { return Intersect(other); }
        IImmutableSet<T> IImmutableSet<T>.Remove(T value) { return Remove(value); }
        IImmutableSet<T> IImmutableSet<T>.SymmetricExcept(IEnumerable<T> other) { return SymmetricExcept(other); }
        IImmutableSet<T> IImmutableSet<T>.Union(IEnumerable<T> other) { return Union(other); }

        void ICollection.CopyTo(Array array, int index) { foreach (var x in _dic.Keys) array.SetValue(x, index++); }
        bool ICollection.IsSynchronized { get { return true; } }
        object ICollection.SyncRoot { get { return this; } }
        void ICollection<T>.Add(T item) { throw new NotSupportedException(); }
        void ICollection<T>.Clear() { throw new NotSupportedException(); }
        bool ICollection<T>.Remove(T item) { throw new NotSupportedException(); }

        bool ISet<T>.Add(T item) { throw new NotSupportedException(); }
        void ISet<T>.ExceptWith(IEnumerable<T> other) { throw new NotSupportedException(); }
        void ISet<T>.IntersectWith(IEnumerable<T> other) { throw new NotSupportedException(); }
        void ISet<T>.SymmetricExceptWith(IEnumerable<T> other) { throw new NotSupportedException(); }
        void ISet<T>.UnionWith(IEnumerable<T> other) { throw new NotSupportedException(); }
    }
}
