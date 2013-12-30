namespace Client
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var client = new Client();
            client.Start(args.Length > 0, "assaultwing.com");
        }
    }
}
