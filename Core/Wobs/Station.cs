using Axiom.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Wobs
{
    [Serializable]
    public class Station : Wob
    {
        private readonly Vector3 _pos;

        public Vector3 Pos { get { return _pos; } }

        public Station(Guid id, Vector3 pos)
            : base(id)
        {
            _pos = pos;
        }

        public override bool Equals(Wob other)
        {
            var station = other as Station;
            return station != null && ID == station.ID && Pos == station.Pos;
        }

        public override string ToString()
        {
            return "Station at " + Pos;
        }
    }
}
