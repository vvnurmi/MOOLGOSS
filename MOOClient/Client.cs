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
    public static class App
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var service = new MOOServiceClient("NetNamedPipeBinding_IMOOService1");
            var app = new Application();
            app.DispatcherUnhandledException += ExceptionHandler;
            app.Run(new ClientWindow(service));
        }

        private static void ExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is CommunicationException)
            {
                MessageBox.Show("There was a communication error. Perhaps the server is offline?\n"
                    + e.Exception.Message, "MOO Communication Error");
                e.Handled = true;
            }
        }
    }
}
