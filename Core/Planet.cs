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
