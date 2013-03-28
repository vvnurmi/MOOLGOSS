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
        private DateTime _now;

        public DateTime Now { get { return _now; } set { _now = value; OnPropertyChanged("Now"); } }
        public List<Planet> Planets { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
