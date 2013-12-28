using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var server = new Server();
            server.Start();
        }
    }
}
