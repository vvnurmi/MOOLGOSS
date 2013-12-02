using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://*:8080/moolgoss/");
            if (!TryStart(listener)) return 1;
            while (true)
            {
                var context = listener.GetContext();
                Console.WriteLine("Contacted from {0}", context.Request.UserHostAddress);
                var data = Encoding.UTF8.GetBytes("Hello there!");
                context.Response.Close(data, true);
            }
        }

        private static bool TryStart(HttpListener listener)
        {
            var prefixes = listener.Prefixes;
            try
            {
                listener.Start();
                return true;
            }
            catch (HttpListenerException ex)
            {
                if (ex.ErrorCode != 5) throw;
                var username = Environment.GetEnvironmentVariable("USERNAME");
                var userdomain = Environment.GetEnvironmentVariable("USERDOMAIN");
                Console.WriteLine("Access limitation. You need to run this first:");
                foreach (var prefix in prefixes)
                    Console.WriteLine("  netsh http add urlacl url={0} user={1}\\{2} listen=yes",
                        prefix, userdomain, username);
                return false;
            }
        }
    }
}
