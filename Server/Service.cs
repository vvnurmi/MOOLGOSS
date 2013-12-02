using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Service : IService
    {
        public Planet[] GetPlanets()
        {
            return new[] { new Planet("Earth") };
        }
    }
}
