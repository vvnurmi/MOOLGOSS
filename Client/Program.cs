using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var service = Marshal.Get<IService>(Invoke);
            var planets = service.GetPlanets();
            Console.WriteLine("Planets:");
            foreach (var planet in planets)
                Console.WriteLine("  {0}", planet.Name);
            Console.WriteLine("\nPress Enter to finish");
            Console.ReadLine();
        }

        private static object Invoke(string name, object[] args)
        {
            var data = Serialization.Break(Tuple.Create(name, args));
            var request = WebRequest.Create("http://localhost:8080/moolgoss/");
            request.Method = "POST";
            request.ContentLength = data.Length;
            using (var requestStream = request.GetRequestStream())
                requestStream.Write(data, 0, data.Length);
            using (var response = request.GetResponse())
            {
                var responseData = new byte[response.ContentLength];
                response.GetResponseStream().Read(responseData, 0, responseData.Length);
                return Serialization.Build<object>(responseData);
            }
        }
    }
}
