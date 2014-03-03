using System;
using System.Linq;

namespace Standalone
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var server = new Server.Server();
            ((Action)server.Start).BeginInvoke(null, null);
            var client = new Client.Client();
            client.Start("localhost",
                userConfigure: args.Contains("--config"),
                debugSettings: args.Contains("--debug"));
        }
    }
}
