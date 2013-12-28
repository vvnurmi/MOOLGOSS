using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Standalone
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var server = new Server.Server();
            ((Action)server.Start).BeginInvoke(null, null);
            var client = new Client.Client();
            client.Start(args.Length > 0, "localhost");
        }
    }
}
