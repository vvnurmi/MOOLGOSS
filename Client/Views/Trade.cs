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
        private bool _resizeByItemCount;

        public bool ResizeByItemCount { get { return _resizeByItemCount; } set { _resizeByItemCount = value; } }
        public OverlayElementContainer TradeElement { get { return _tradeElement; } }

        public Trade(string name, int width, int height)
            : base(name, width - 20, height)
        {
            _name = name;
            _tradeWidth = width;
            _tradeHeight = height;
            _tradeElement = CreateTradeElement();
            ListElement.Left = 6;
            ListElement.Top = 6;
            TradeContent.AddChildElement(ListElement);
            ResizeTradeElement();
        }

        private OverlayElementContainer CreateTradeElement()
        {
            var tradeElement = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/Trade", null, InstanceName);
            return tradeElement;
        }

        private void ResizeTradeElement()
        {
            var height = _tradeHeight;
            if (ResizeByItemCount)
                height = (int)Math.Round(ItemCount * ListItemTemplate.Height + 20);
            _tradeElement.Width = _tradeWidth;
            _tradeElement.Height = height;
            TradeContent.Width = _tradeElement.Width - TradeContent.Left * 2;
            TradeContent.Height = _tradeElement.Height - TradeContent.Top * 2;
        }

        public override void Destroy()
        {
            TradeContent.RemoveChild(ListElement.Name);
            base.Destroy();
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
    }
}