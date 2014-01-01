using Axiom.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
