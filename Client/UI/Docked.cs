using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UI
{
    class Docked : UIMode
    {
        public Docked()
            : base("Docked")
        {
            Enter = EnterHandler;
            Update = UpdateHandler;
            Exit = ExitHandler;
        }

        private void EnterHandler()
        {
        }

        private void UpdateHandler(float secondsPassed)
        {
        }

        private void ExitHandler()
        {
        }
    }
}
