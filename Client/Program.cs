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
            var request = WebRequest.Create("http://localhost:8080/moolgoss/");
            using (var response = request.GetResponse())
            {
                var stream = new StreamReader(response.GetResponseStream());
                Console.WriteLine(stream.ReadToEnd());
            }
            Console.WriteLine("Press Enter to finish");
            Console.ReadLine();
        }
    }
}
