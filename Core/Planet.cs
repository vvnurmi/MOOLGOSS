using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Planet : IEntity
    {
        [Prop]
        public string Name { get; private set; }

        public Planet(string name)
        {
            Name = name;
        }
    }
}
