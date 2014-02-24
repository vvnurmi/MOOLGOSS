using Axiom.Input;
using Axiom.Overlays;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TopBarView = Client.Views.TopBar;
using InteractionView = Client.Views.List;
using DialogueView = Client.Views.Dialogue;
using TradingView = Client.Views.List;

namespace Client.UI
{
    class Docked : UIMode
    {
        private string LeftVerticalBarInstanceName { get { return "Overlays/Elements/LeftVerticalBarInstance/Docked"; } }
        private TopBarView _topBarView;
        private InteractionView _interactionView;
        private DialogueView _dialogueView;
        private TradingView _tradingView;
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

            _interactionView.AddItem("TestInstance", "Portrait", "[RK] Gom", "Guild Master", "Small", "Default", "DEAD", InteractionListElementClicked);
            _interactionView.AddItem("TestInstance2", "Portrait", "[RK] Chapelier", "Lord of The Code", "Small", "Default", "", InteractionListElementClicked);

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
            if (_interactionView != null)
            {
                _interactionView.RemoveItem("TestInstance");
                _interactionView.RemoveItem("TestInstance2");
            }

            DestroyDialogueView();

            if (_leftVerticalBar.IsVisible)
            {
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
            DestroyDialogueView();
            _dialogueView = CreateDialogueView();
            _dialogueView.DialogueElement.Left = _interactionView.ListElement.Width + 30;
            _dialogueView.DialogueElement.VerticalAlignment = VerticalAlignment.Center;
            _dialogueView.DialogueElement.Top = -(float)Math.Round(_dialogueView.DialogueElement.Height / 2);
            _leftVerticalBarElement.AddChildElement(_dialogueView.DialogueElement);
        }

        private OverlayElementContainer CreateLeftVerticalBarElement()
        {
            var leftVerticalBar = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/DockedLeftVerticalBar", null, LeftVerticalBarInstanceName);
            return leftVerticalBar;
        }

        private DialogueView CreateDialogueView()
        {
            if (_dialogueView == null)
            {
                var dialogueView = new DialogueView("DockedDialogue", 310, 600);
                dialogueView.AddButton("history", "Blue", "History (Event log)", DialogueViewOptionClicked);
                dialogueView.AddOption("quit", "Bye Bye!", DialogueViewOptionClicked);
                dialogueView.AddOption("buy", "Can I buy something?", ShowTrading);
                dialogueView.AddOption("chape", "Tell me about Chapelier!", DialogueViewOptionClicked);
                dialogueView.AddProperty("name", "NAME:", "Gom", DialogueViewOptionClicked);
                dialogueView.AddProperty("faction", "FACTION:", "Reilu Kerho [RK]", DialogueViewOptionClicked);
                dialogueView.AddProperty("title", "TITLE:", "Guild Master", DialogueViewOptionClicked);
                dialogueView.SetPortrait("Default");
                return dialogueView;
            }

            return _dialogueView;
        }

        private void DestroyDialogueView()
        {
            if (_dialogueView != null)
            {
                DestroyTradingView();

                _leftVerticalBarElement.RemoveChild(_dialogueView.DialogueElement.Name);
                _dialogueView.RemoveButton("history");
                _dialogueView.RemoveOption("buy");
                _dialogueView.RemoveOption("quit");
                _dialogueView.RemoveOption("chape");
                _dialogueView.RemoveProperty("name");
                _dialogueView.RemoveProperty("faction");
                _dialogueView.RemoveProperty("title");
                _dialogueView.Destroy();
                _dialogueView = null;
            }
        }

        private void DialogueViewOptionClicked()
        {
            DestroyDialogueView();
        }

        private void ShowTrading()
        {
            DestroyTradingView();
            _tradingView = CreateTradingView();
            _tradingView.ListElement.Left = _dialogueView.DialogueElement.Left + _dialogueView.DialogueElement.Width;
            _tradingView.ListElement.VerticalAlignment = VerticalAlignment.Center;
            _tradingView.ListElement.Top = -(float)Math.Round(_tradingView.ListElement.Height / 2);
            _leftVerticalBarElement.AddChildElement(_tradingView.ListElement);
        }

        private TradingView CreateTradingView()
        {
            var tradingView = new TradingView("trading", 310, 600);
            tradingView.AddItem("trade1", "Material", "Iron ore", "It's always needed...", "Solid", "IronOre", "", InteractionListElementClicked);
            return tradingView;
        }

        private void DestroyTradingView()
        {
            if (_tradingView != null)
            {
                _leftVerticalBarElement.RemoveChild(_tradingView.ListElement.Name);
                _tradingView.RemoveItem("trade1");
                _tradingView.Destroy();
                _tradingView = null;
            }
        }
    }
}
