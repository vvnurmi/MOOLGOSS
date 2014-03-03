using System.Linq;

namespace Client
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var client = new Client();
            client.Start("assaultwing.com",
                userConfigure: args.Contains("--config"),
                debugSettings: args.Contains("--debug"));
        }
    }
}
