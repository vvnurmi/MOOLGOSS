using Axiom.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Wobs
{
    [Serializable]
    public sealed class Planet : Wob
    {
        private readonly string _name;

        public string Name { get { return _name; } }
        public Vector3 Pos { get { return new Vector3(75 * Name.Length, 0, 75 * (Name[0] - 'A') ); } }

        public Planet(Guid id, string name)
            : base(id)
        {
            _name = name;
        }

        public override bool Equals(Wob other)
        {
            var planet = other as Planet;
            return planet != null && ID == planet.ID && Name == planet.Name && Pos == planet.Pos;
        }

        public override string ToString()
        {
            return "Planet " + Name;
        }
    }
}
