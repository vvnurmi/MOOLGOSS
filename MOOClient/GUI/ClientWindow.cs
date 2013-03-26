using MOO.Client.MOOService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MOO.Client.GUI
{
    public partial class ClientWindow : Window
    {
        private MOOServiceClient _service;
        private Planet[] _planets = new Planet[0];

        public ClientWindow(MOOServiceClient service)
        {
            _service = service;
            Background = Brushes.Black;
            Content = new TextBlock(new Run("Hello")) { Foreground = Brushes.White };
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                _planets = _service.GetPlanets();
            base.OnKeyDown(e);
        }
    }
}
