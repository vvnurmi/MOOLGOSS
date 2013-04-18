using MOO.Client.GUI;
using MOO.Client.MOOService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

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
            DispatcherUnhandledException += ExceptionHandler;
            _window = new ClientWindow(CreateService, _state);
        }

        private MOOServiceClient CreateService(string player)
        {
            return TryCreateService(player, "BasicHttpBinding_IMOOService")
                ?? TryCreateService(player, "NetNamedPipeBinding_IMOOService");
        }

        private MOOServiceClient TryCreateService(string player, string configuration)
        {
            var client = new MOOServiceClient(configuration);
            try
            {
                client.Authenticate(player);
                return client;
            }
            catch (CommunicationException)
            {
                return null;
            }
        }

        private void ExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is CommunicationException)
            {
                e.Handled = true;
                _window.AbandonServer();
            }
        }
    }
}
