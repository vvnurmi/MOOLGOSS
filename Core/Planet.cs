using Axiom.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    [Serializable]
    public class Planet
    {
        public string Name { get; private set; }
        public Vector3 Pos { get { return new Vector3(75 * Name.Length, 0, 75 * (Name[0] - 'A') ); } }

        public Planet(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return "Planet " + Name;
        }
    }
}
