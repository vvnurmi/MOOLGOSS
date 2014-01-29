using Axiom.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UI
{
    internal class TitleScreen : UIMode
    {
        public TitleScreen()
            : base("Title Screen")
        {
            Enter = EnterHandler;
            Update = UpdateHandler;
            Exit = ExitHandler;
        }

        private void EnterHandler()
        {
            Globals.UI.ShowTitleScreen();
        }

        private void UpdateHandler(float secondsPassed)
        {
            if (Globals.Input.IsKeyDownEvent(KeyCodes.Enter) || Globals.Input.IsKeyDownEvent(KeyCodes.Space))
                Globals.UI.SetMode("Gameplay");
        }

        private void ExitHandler()
        {
            Globals.UI.HideTitleScreen();
        }
    }
}
