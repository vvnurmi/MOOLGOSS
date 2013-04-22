using MOO.Client.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Thrift;
using Thrift.Protocol;
using Thrift.Transport;

namespace MOO.Client
{
    public class App : Application
    {
        private ClientWindow _window;
        private State _state;

        [STAThread]
        public static void Main(string[] args)
        {
            var app = new App();
            app.Run(app._window);
        }

        private App()
        {
            _state = new State();
            _window = new ClientWindow(CreateService, _state);
        }

        private MOO.Service.MOOService.Client CreateService(string player)
        {
            try
            {
                var transport = new TSocket("localhost", 8000);
                var protocol = new TBinaryProtocol(transport);
                transport.Open();
                var client = new MOO.Service.MOOService.Client(protocol);
                client.Authenticate(player);
                return client;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
