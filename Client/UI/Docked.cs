using Axiom.Input;
using Axiom.Overlays;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TopBarView = Client.Views.TopBar;
using InteractionView = Client.Views.SimpleList;

namespace Client.UI
{
    class Docked : UIMode
    {
        private string LeftVerticalBarInstanceName { get { return "Overlays/Elements/LeftVerticalBarInstance/Docked"; } }
        private TopBarView _topBarView;
        private InteractionView _interactionView;
        private string _baseName = "First Space Station";
        private string _currentLocation;
        private OverlayElementContainer _leftVerticalBarElement;
        private Overlay _leftVerticalBar;

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

            if (_interactionView == null)
            {
                _interactionView = new InteractionView("Docked", 320, 500);
            }

            _interactionView.AddItem("TestInstance", "[RK] Gom", "Guild Master", "Solid", "IronOre", "DEAD", InteractionListElementClicked);
            _interactionView.AddItem("TestInstance2", "[RK] Chapelier", "Lord of The Code", "Gas", "Hydrogen", "", InteractionListElementClicked);

            if (_leftVerticalBar == null)
            {
                _leftVerticalBar = OverlayManager.Instance.Create("Overlays/LeftVerticalBar/Docked");
                _leftVerticalBarElement = CreateLeftVerticalBarElement();
                _leftVerticalBarElement.AddChildElement(_interactionView.ListElement);
                _interactionView.ListElement.VerticalAlignment = VerticalAlignment.Center;
                _interactionView.ListElement.Top = -(float)Math.Round(_interactionView.ListElement.Height / 2);
                _leftVerticalBar.AddElement(_leftVerticalBarElement);
            }

            _topBarView.AddButton("bar", "BAR (F1)", MoveToBar);
            _topBarView.AddButton("shopping", "SHOPS (F2)", MoveToShopping);
            _topBarView.AddButton("hangar", "HANGAR (F3)", MoveToHangar);
            _topBarView.AddButton("leave", "LEAVE (F4)", Leave);

            MoveToHangar();
        }

        private void UpdateHandler(float secondsPassed)
        {
            if (!_topBarView.IsVisible)
                _topBarView.Show();
            if (!_leftVerticalBar.IsVisible)
                _leftVerticalBar.Show();
            if (Globals.Input.IsKeyDownEvent(KeyCodes.F1))
                MoveToBar();
            if (Globals.Input.IsKeyDownEvent(KeyCodes.F2))
                MoveToShopping();
            if (Globals.Input.IsKeyDownEvent(KeyCodes.F3))
                MoveToHangar();
            if (Globals.Input.IsKeyDownEvent(KeyCodes.F4))
                Leave();
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
            if (_leftVerticalBar.IsVisible)
            {
                _interactionView.RemoveItem("TestInstance");
                _interactionView.RemoveItem("TestInstance2");
                _leftVerticalBar.Hide();
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

        private void Leave()
        {
            Globals.UI.SetMode("Gameplay");
        }

        private void InteractionListElementClicked()
        {
        }

        private OverlayElementContainer CreateLeftVerticalBarElement()
        {
            var leftVerticalBar = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/DockedLeftVerticalBar", null, LeftVerticalBarInstanceName);
            return leftVerticalBar;
        }
    }
}
