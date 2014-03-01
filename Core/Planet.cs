using Axiom.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    [Serializable]
    public class Planet : IEquatable<Planet>
    {
        private readonly Guid _id;
        private readonly string _name;

        public Guid ID { get { return _id; } }
        public string Name { get { return _name; } }
        public Vector3 Pos { get { return new Vector3(75 * Name.Length, 0, 75 * (Name[0] - 'A') ); } }

        public Planet(Guid id, string name)
        {
            _id = id;
            _name = name;
        }

        public bool Equals(Planet other)
        {
            return ID == other.ID && Name == other.Name && Pos == other.Pos;
        }

        public override string ToString()
        {
            return "Planet " + Name;
        }
    }
}
