using Axiom.Math;
using Core;
using Core.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Server
    {
        public void Start()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://*:8080/moolgoss/");
            if (!TryStart(listener)) return;
            var service = new Service();
            service.AddStation(Guid.NewGuid(), new Vector3(200, 0, 100));
            var marshalledService = Marshal.Get(service);
            while (true)
                try
                {
                    var context = listener.GetContext();
                    var data = new byte[context.Request.ContentLength64];
                    context.Request.InputStream.Read(data, 0, data.Length);
                    var callspec = Serialization.Build<MarshalledCall>(data);
                    var result = marshalledService.Invoke(callspec.Name, callspec.Args);
                    if (result != null)
                        context.Response.Close(Serialization.Break(result), true);
                    else
                        context.Response.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("[{0:u}] Exception: {1}", DateTime.Now, e);
                }
        }

        private bool TryStart(HttpListener listener)
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
                Console.WriteLine("Access limitation. If you're on Windows, you need to run this as Administrator:");
                foreach (var prefix in prefixes)
                    Console.WriteLine("  netsh http add urlacl url={0} user={1}\\{2} listen=yes",
                        prefix, userdomain, username);
                return false;
            }
        }
    }
}
