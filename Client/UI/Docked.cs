using Axiom.Input;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TopBarView = Client.Views.TopBar;

namespace Client.UI
{
    class Docked : UIMode
    {
        private TopBarView _topBarView;
        private string _baseName = "First Space Station";
        private string _currentLocation;

        public Docked()
            : base("Docked")
        {
            Enter = EnterHandler;
            Update = UpdateHandler;
            Exit = ExitHandler;
        }

        private void EnterHandler()
        {
            Globals.UI.ShowMouse();

            if (_topBarView == null)
            {
                _topBarView = new TopBarView("Docked", _baseName);
            }

            _topBarView.AddButton("bar", "BAR (F1)", MoveToBar);
            _topBarView.AddButton("shopping", "SHOPS (F2)", MoveToShopping);
            _topBarView.AddButton("hangar", "HANGAR (F3)", MoveToHangar);
            _topBarView.AddButton("leave", "LEAVE (F4)", TryLeave);

            MoveToHangar();
        }

        private void UpdateHandler(float secondsPassed)
        {
            if (!_topBarView.IsVisible)
                _topBarView.Show();
            if (Globals.Input.IsKeyDownEvent(KeyCodes.F1))
                MoveToBar();
            if (Globals.Input.IsKeyDownEvent(KeyCodes.F2))
                MoveToShopping();
            if (Globals.Input.IsKeyDownEvent(KeyCodes.F3))
                MoveToHangar();
            if (Globals.Input.IsKeyDownEvent(KeyCodes.F4))
                TryLeave();
        }

        private void ExitHandler()
        {
            if (_topBarView.IsVisible)
            {
                _topBarView.RemoveButton("bar");
                _topBarView.RemoveButton("shopping");
                _topBarView.RemoveButton("hangar");
                _topBarView.RemoveButton("leave");
                _topBarView.Hide();
            }
        }

        private void MoveTo(string location)
        {
            _currentLocation = location;

            if (_topBarView != null)
                _topBarView.SetLocationText(_baseName + " : " + _currentLocation);
        }

        private void MoveToBar()
        {
            MoveTo("Bar");
        }

        private void MoveToShopping()
        {
            MoveTo("Shops");
        }

        private void MoveToHangar()
        {
            MoveTo("Hangar");
        }

        private void TryLeave()
        {
            Globals.UI.SetMode("Gameplay");
        }
    }
}
