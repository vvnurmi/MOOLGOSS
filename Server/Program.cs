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
        public static int Main(string[] args)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://*:8080/moolgoss/");
            if (!TryStart(listener)) return 1;
            var service = new Service();
            var marshalledService = Marshal.Get(service);
            while (true)
            {
                var context = listener.GetContext();
                var data = new byte[context.Request.ContentLength64];
                context.Request.InputStream.Read(data, 0, data.Length);
                var callspec = Serialization.Build<Tuple<string, object[]>>(data);
                var result = marshalledService.Invoke(callspec.Item1, callspec.Item2);
                if (result != null)
                    context.Response.Close(Serialization.Break(result), true);
                else
                    context.Response.Close();
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
