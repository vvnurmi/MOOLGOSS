using Axiom.Math;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public static class Util
    {
        /// <summary>
        /// Interpolates between two quaternions, using spherical linear interpolation along the shortest path.
        /// </summary>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <remarks>
        /// Taken from the SlimMath library by the SlimDX group.
        /// Avoids a bug that is present in <see cref="Axiom.Math.Quaternion.Slerp"/>.
        /// </remarks>
        public static Quaternion SlerpShortest(float amount, Quaternion start, Quaternion end)
        {
            float opposite;
            float inverse;
            float dot = start.Dot(end);
            if (Math.Abs(dot) > 1 - 1e-3f)
            {
                inverse = 1 - amount;
                opposite = amount * Math.Sign(dot);
            }
            else
            {
                var acos = (float)Utility.ACos(Utility.Abs(dot));
                var invSin = 1.0 / Utility.Sin(acos);
                inverse = Utility.Sin((1 - amount) * acos) * invSin;
                opposite = Utility.Sin(amount * acos) * invSin * Utility.Sign(dot);
            }
            return new Quaternion(
                inverse * start.w + opposite * end.w,
                inverse * start.x + opposite * end.x,
                inverse * start.y + opposite * end.y,
                inverse * start.z + opposite * end.z);
        }

        public static bool ValueEquals<K, V>(this IEnumerable<KeyValuePair<K, V>> a, IEnumerable<KeyValuePair<K, V>> b) where V : IEquatable<V>
        {
            // 3M keys, release build: 900 s
            //return a.Count() == b.Count() && !a.Except(b).Any();
            //return true;

            // 3M keys, release build: 8.0 s
            //var aSorted = a.OrderBy(x => x.Key);
            //var bSorted = b.OrderBy(x => x.Key);
            //if (aSorted.Count() != bSorted.Count()) return false;
            //foreach (var pair in aSorted.Zip(bSorted, (q, w) => new KeyValuePair<KeyValuePair<K, V>, KeyValuePair<K, V>>(q, w)))
            //    if (!pair.Key.Equals(pair.Value)) return false;
            //return true;

            // 3M keys, release build: 4.3 s
            var aSorted = a.OrderBy(x => x.Key).ToArray();
            var bSorted = b.OrderBy(x => x.Key).ToArray();
            if (aSorted.Length != bSorted.Length) return false;
            for (int i = 0; i < aSorted.Length; i++)
                if (!aSorted[i].Equals(bSorted[i])) return false;
            return true;
        }

        public static bool ValueEquals<K, V>(this IDictionary<K, V> a, IDictionary<K, V> b) where V : IEquatable<V>
        {
            // 3M keys, release build: 0.33 s
            if (a.Count != b.Count) return false;
            foreach (var x in a)
            {
                V bValue;
                if (!b.TryGetValue(x.Key, out bValue)) return false;
                if (!x.Value.Equals(bValue)) return false;
            }
            return true;
        }
    }
}
