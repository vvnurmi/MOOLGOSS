using Axiom.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    [Serializable]
    public class Station : IEquatable<Station>
    {
        public Guid ID { get; private set; }
        public Vector3 Pos { get; set; }

        public Station(Guid id)
        {
            ID = id;
        }

        public bool Equals(Station other)
        {
            return ID == other.ID && Pos == other.Pos;
        }

        public override string ToString()
        {
            return "Station at " + Pos;
        }
    }
}
