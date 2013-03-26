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

namespace MOO.Client
{
    public static class App
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var service = new MOOServiceClient("NetNamedPipeBinding_IMOOService1");
            new Application().Run(new ClientWindow(service));
        }
    }
}
