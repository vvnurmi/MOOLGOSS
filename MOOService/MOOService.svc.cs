using MOO.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace MOO.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class MOOService : IMOOService
    {
        public Planet[] GetPlanets()
        {
            return new[]
            {
                new Planet(id : 42, name : "Foo", maxPopulation : 99, population : 69, orbit : 3)
            };
        }
    }
}
