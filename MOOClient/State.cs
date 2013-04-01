using MOO.Client.MOOService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.Client
{
    public class State : INotifyPropertyChanged
    {
        private DateTime _stardate;

        public DateTime Stardate { get { return _stardate; } set { _stardate = value; OnPropertyChanged("Stardate"); } }
        public List<Planet> Planets { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
