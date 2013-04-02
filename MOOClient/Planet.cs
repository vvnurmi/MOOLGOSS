using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.Client.MOOService
{
    public partial class Planet
    {
        public override bool Equals(object obj)
        {
            var planet = obj as Planet;
            if (planet == null) return false;
            return
                planet.ID == ID &&
                planet.Population == Population &&
                planet.MaxPopulation == MaxPopulation &&
                planet.Player == Player;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }
}
