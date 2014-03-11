using Axiom.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Wobs
{
    [Serializable]
    public class Droid : Ship
    {
        public Droid(Guid id, Vector3 pos, Vector3 front, Vector3 up)
            : base(id, pos, front, up)
        {
        }

        public override bool Equals(Wob other)
        {
            var droid = other as Droid;
            return droid != null && base.Equals(droid);
        }

    }
}
