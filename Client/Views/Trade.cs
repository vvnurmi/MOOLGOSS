using Axiom.Overlays;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Views
{
    class Trade : List
    {
        private string InstanceName { get { return "Overlays/Elements/Trade/" + _name; } }
        private OverlayElementContainer TradeContent { get { return (OverlayElementContainer)_tradeElement.GetChild(InstanceName + "/TradeContent"); } }
        private string _name;
        private OverlayElementContainer _tradeElement;
        private int _tradeWidth;
        private int _tradeHeight;
        private OverlayElementContainer _closeButton;
        private OverlayElementContainer _confirmButton;
        private bool _resizeByItemCount;

        public bool ResizeByItemCount { get { return _resizeByItemCount; } set { _resizeByItemCount = value; } }
        public OverlayElementContainer TradeElement { get { return _tradeElement; } }

        public Trade(string name, int width, int height)
            : base(name, width - 20, height)
        {
            _name = name;
            _tradeWidth = width;
            _tradeHeight = height;
            AddTradeElement();
            ResizeTradeElement();
        }

        private OverlayElementContainer CreateTradeElement()
        {
            var tradeElement = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/Trade", null, InstanceName);
            return tradeElement;
        }

        private void AddTradeElement()
        {
            _tradeElement = CreateTradeElement();
            ListElement.Left = 6;
            ListElement.Top = 6;
            TradeContent.AddChildElement(ListElement);
        }

        private void ResizeTradeElement()
        {
            var height = _tradeHeight;
            if (ResizeByItemCount)
            {
                height = (int)Math.Round(ItemCount * ListItemTemplate.Height + 22);
                if (_closeButton != null || _confirmButton != null)
                    height += 30;
            }
            _tradeElement.Width = _tradeWidth;
            _tradeElement.Height = height;
            TradeContent.Width = _tradeElement.Width - TradeContent.Left * 2;
            TradeContent.Height = _tradeElement.Height - TradeContent.Top * 2;
        }

        private void AddConfirmButton()
        {
            _confirmButton = CreateConfirmButton();
            _confirmButton.VerticalAlignment = VerticalAlignment.Bottom;
            _confirmButton.HorizontalAlignment = HorizontalAlignment.Right;
            _confirmButton.Left = -(_confirmButton.Width + 6);
            _confirmButton.Top = -(_confirmButton.Height + 6);
            TradeContent.AddChildElement(_confirmButton);
        }

        private OverlayElementContainer CreateConfirmButton()
        {
            var confirmButton = Globals.UI.CreateButton(InstanceName + "/ConfirmButton", "Green", "Accept", (int)TradeContent.Width / 2 - 9, 25, 18);
            return confirmButton;
        }

        private void DestroyConfirmButton()
        {
            if (_confirmButton != null)
            {
                TradeContent.RemoveChild(_confirmButton.Name);
                if (_confirmButton.UserData != null)
                    Globals.UI.RemoveButton(_confirmButton);
                Globals.UI.DestroyButton(_confirmButton.Name);
            }
        }

        private void AddCloseButton()
        {
            _closeButton = CreateCloseButton();
            _closeButton.VerticalAlignment = VerticalAlignment.Bottom;
            _closeButton.HorizontalAlignment = HorizontalAlignment.Left;
            _closeButton.Left = 6;
            _closeButton.Top = -(_closeButton.Height + 6);
            TradeContent.AddChildElement(_closeButton);
        }

        private OverlayElementContainer CreateCloseButton()
        {
            var closeButton = Globals.UI.CreateButton(InstanceName + "/CloseButton", "Blue", "Close", (int)TradeContent.Width / 2 - 9, 25, 18);
            return closeButton;
        }

        private void DestroyCloseButton()
        {
            if (_closeButton != null)
            {
                TradeContent.RemoveChild(_closeButton.Name);
                if (_closeButton.UserData != null)
                    Globals.UI.RemoveButton(_closeButton);
                Globals.UI.DestroyButton(_closeButton.Name);
            }
        }

        public override void Destroy()
        {
            TradeContent.RemoveChild(ListElement.Name);
            base.Destroy();
            DestroyConfirmButton();
            DestroyCloseButton();
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/TradeContent/BorderBL");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/TradeContent/BorderTR");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/TradeContent");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/TechTL");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/TechBL");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/TechTR");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/TechBR");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName);
        }

        public override void AddItem(string instanceName, string type, string header, string description, string iconCategory, string iconName, string iconLabel, Action action)
        {
            base.AddItem(instanceName, type, header, description, iconCategory, iconName, iconLabel, action);
            ResizeTradeElement();
        }

        public void ShowButtons()
        {
            if (_confirmButton == null)
                AddConfirmButton();
            if (_closeButton == null)
                AddCloseButton();

            ResizeTradeElement();
        }

        public void HideButtons()
        {
            DestroyConfirmButton();
            DestroyCloseButton();
        }

        public void SetCloseButtonAction(Action action)
        {
            if (_closeButton != null)
            {
                if (_closeButton.UserData != null)
                    Globals.UI.RemoveButton(_closeButton);
                _closeButton.UserData = action;
                Globals.UI.AddButton(_closeButton);
            }
        }

        public void SetConfirmButtonAction(Action action)
        {
            if (_confirmButton != null)
            {
                if (_confirmButton.UserData != null)
                    Globals.UI.RemoveButton(_confirmButton);
                _confirmButton.UserData = action;
                Globals.UI.AddButton(_confirmButton);
            }
        }
    }
}