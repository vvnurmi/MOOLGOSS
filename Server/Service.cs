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
        public string HelloWorld()
        {
            return "Hello world!";
        }
    }
}
