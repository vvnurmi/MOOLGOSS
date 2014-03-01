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
using TradeView = Client.Views.Trade;
using VendorList = Client.Views.Trade;

namespace Client.UI
{
    class Docked : UIMode
    {
        private string LeftVerticalBarInstanceName { get { return "Overlays/Elements/LeftVerticalBarInstance/Docked"; } }
        private TopBarView _topBarView;
        private InteractionView _interactionView;
        private DialogueView _dialogueView;
        private VendorList _vendorList;
        private TradeView _tradeView;
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

            _interactionView.AddHeader("people", "PEOPLE");
            _interactionView.AddItem("TestInstance", "Portrait", "[RK] Gom", "Guild Master", "Small", "Default", "", InteractionListElementClicked);
            _interactionView.AddItem("TestInstance2", "Portrait", "[RK] Chapelier", "Lord of The Code", "Small", "Default", "", InteractionListElementClicked);
            _interactionView.AddHeader("ships", "SHIPS");
            _interactionView.AddItem("TestShip", "Portrait", "Nautilus", "Small Fighter", "Small", "Default", "", InteractionListElementClicked);

            if (_leftVerticalBar == null)
            {
                _leftVerticalBar = OverlayManager.Instance.Create("Overlays/LeftVerticalBar/Docked");
                _leftVerticalBarElement = CreateLeftVerticalBarElement();
                _leftVerticalBarElement.AddChildElement(_interactionView.ListElement);
                _interactionView.ListElement.VerticalAlignment = VerticalAlignment.Center;
                _interactionView.ListElement.Top = -(_interactionView.AbsoluteContentHeight / 2);
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
                _interactionView.RemoveItem("TestShip");
                _interactionView.RemoveHeader("people");
                _interactionView.RemoveHeader("ships");
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
                dialogueView.AddOption("buy", "Can I buy something?", ShowVendorList);
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
                DestroyVendorList();
                DestroyTradeView();

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

        private void ShowVendorList()
        {
            DestroyTradeView();
            DestroyVendorList();
            _vendorList = CreateVendorList();
            _vendorList.TradeElement.Left = _dialogueView.DialogueElement.Left + _dialogueView.DialogueElement.Width;
            _vendorList.TradeElement.VerticalAlignment = VerticalAlignment.Center;
            _vendorList.TradeElement.Top = -(float)Math.Round(_vendorList.TradeElement.Height / 2);
            _leftVerticalBarElement.AddChildElement(_vendorList.TradeElement);
        }

        private VendorList CreateVendorList()
        {
            var vendorList = new VendorList("trading", 310, 600);
            vendorList.AddItem("trade1", "Material", "Iron ore", "It's always needed...", "Solid", "IronOre", "40", VendorListElementClicked);
            return vendorList;
        }

        private void DestroyVendorList()
        {
            if (_vendorList != null)
            {
                _leftVerticalBarElement.RemoveChild(_vendorList.TradeElement.Name);
                _vendorList.RemoveItem("trade1");
                _vendorList.Destroy();
                _vendorList = null;
            }
        }

        private void VendorListElementClicked()
        {
            ShowTradeView();
        }

        private void ShowTradeView()
        {
            DestroyTradeView();
            _tradeView = CreateTradeView();
            _tradeView.TradeElement.Left = _vendorList.TradeElement.Left + _vendorList.TradeElement.Width + 6;
            _tradeView.TradeElement.VerticalAlignment = VerticalAlignment.Center;
            _tradeView.TradeElement.Top = -((float)Math.Round(_vendorList.TradeElement.Height / 2) + 26);
            _leftVerticalBarElement.AddChildElement(_tradeView.TradeElement);
        }

        private TradeView CreateTradeView()
        {
            var trade = new TradeView("buyitem", 310, 600);
            trade.ResizeByItemCount = true;
            trade.AddItem("trade1-1", "Material", "Hydrogen", "Lucky you don't fart helium..", "Gas", "Hydrogen", "10", VendorListElementClicked);
            trade.AddItem("trade1-2", "Material", "Dilithium Crystals", "Wow, Such Power", "Energy", "Dilithium", "2", VendorListElementClicked);
            trade.ShowButtons();
            trade.SetCloseButtonAction(DestroyTradeView);
            trade.SetConfirmButtonAction(DestroyTradeView);
            return trade;
        }

        private void DestroyTradeView()
        {
            if (_tradeView != null)
            {
                _leftVerticalBarElement.RemoveChild(_tradeView.TradeElement.Name);
                _tradeView.RemoveItem("trade1-1");
                _tradeView.RemoveItem("trade1-2");
                _tradeView.Destroy();
                _tradeView = null;
            }
        }
    }
}
