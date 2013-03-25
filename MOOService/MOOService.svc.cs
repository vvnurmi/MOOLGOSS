using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace MOO.Service
{
    public class MOOService : IMOOService
    {
        public Planet[] GetPlanets()
        {
            return new[]
            {
                new Planet { ID = 42, Name = "Foo", MaxPopulation = 99, Population = 69, Orbit = 3 }
            };
        }
    }
}
