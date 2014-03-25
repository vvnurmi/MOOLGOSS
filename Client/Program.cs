using System;
using System.Linq;

namespace Client
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var client = new Client();
            var hostIndex = Array.IndexOf(args, "--host");
            var host = hostIndex != -1 && hostIndex + 1 < args.Length
                ? args[hostIndex + 1]
                : "assaultwing.com";
            client.Start(host,
                userConfigure: args.Contains("--config"),
                debugSettings: args.Contains("--debug"));
        }
    }
}
