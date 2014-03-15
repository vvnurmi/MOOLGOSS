using Axiom.Math;
using System;
using System.Collections.Generic;
using System.IO;
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

        public static bool ValueEquals<K, V>(this IReadOnlyDictionary<K, V> a, IReadOnlyDictionary<K, V> b) where V : IEquatable<V>
        {
            if (a.Count != b.Count) return false;
            foreach (var x in a)
            {
                V bValue;
                if (!b.TryGetValue(x.Key, out bValue)) return false;
                if (!x.Value.Equals(bValue)) return false;
            }
            return true;
        }

        /// <summary>
        /// Reads bytes from the stream into the buffer until the buffer is full or the stream is exhausted.
        /// </summary>
        public static void ReadTo(this Stream inputStream, byte[] buffer)
        {
            var offset = 0;
            while (true)
            {
                var readCount = inputStream.Read(buffer, offset, buffer.Length - offset);
                if (readCount == 0) break;
                offset += readCount;
            }
        }

        /// <summary>
        /// Returns the element for which <paramref name="valuator"/> returns the smallest value.
        /// </summary>
        public static T MinBy<T, V>(this IEnumerable<T> seq, Func<T, V> valuator) where V : IComparable
        {
            return seq.Aggregate((best, x) => best == null || valuator(best).CompareTo(valuator(x)) > 0 ? x : best);
        }
    }
}
