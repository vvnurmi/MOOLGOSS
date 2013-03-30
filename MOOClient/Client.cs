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

        private MOOServiceClient CreateService()
        {
            var callbackHandler = new MOOCallbackHandler(_state);
            var instanceContext = new InstanceContext(callbackHandler);
            return new MOOServiceClient(instanceContext, "NetNamedPipeBinding_IMOOService");
        }

        private void ExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is CommunicationException)
            {
                MessageBox.Show("There was a communication error. Perhaps the server is offline?\n"
                    + e.Exception.Message, "MOO Communication Error");
                e.Handled = true;
                _window.ServerButton.IsChecked = false;
            }
        }
    }
}
