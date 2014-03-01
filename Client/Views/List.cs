using Axiom.Overlays;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Views
{
    class List
    {
        private string InstanceName { get { return "Overlays/Elements/List/" + _name; } }
        private OverlayElementContainer _listElement;
        private string _name;
        private int _listItemCount = 0;
        private int _listItemVerticalMargin = 2;
        private int _headerCount = 0;
        private int _headerTopMargin = 4;
        private int _listWidth = 0;
        private int _listHeight = 0;

        public OverlayElementContainer ListElement { get { return _listElement; } }
        public int ItemCount { get { return _listItemCount; } }
        public OverlayElement ListItemTemplate { get { return OverlayManager.Instance.Elements.GetElement("Overlays/Templates/ListItem", true); } }
        public OverlayElement ListHeaderTemplate { get { return OverlayManager.Instance.Elements.GetElement("Overlays/Templates/ListHeader", true); } }
        public int ListItemsContentHeight { get { return (int)Math.Round(_listItemCount * (ListItemTemplate.Height + _listItemVerticalMargin)); } }
        public int HeadersContentHeight { get { return (int)Math.Round(_headerCount * (ListHeaderTemplate.Height + _headerTopMargin)); } }
        public int AbsoluteContentHeight { get { return ListItemsContentHeight + HeadersContentHeight; } }

        public List(string name, int width, int height)
        {
            _name = name;
            _listWidth = width;
            _listElement = CreateListElement(_listWidth, 50);
        }

        public virtual void Destroy()
        {
            OverlayManager.Instance.Elements.DestroyElement(InstanceName);
        }

        public virtual void AddItem(string instanceName, string type, string header, string description, string iconCategory, string iconName, string iconLabel, Action action)
        {
            var listItemBase = CreateListItem(instanceName, type, header, description, iconCategory, iconName, iconLabel);
            _listElement.AddChild(listItemBase);
            listItemBase.UserData = action;
            Globals.UI.AddButton(listItemBase);
            _listItemCount++;
            ResizeListElement();
        }

        public virtual void RemoveItem(string instanceName)
        {
            DestroyListItem(instanceName);
            _listItemCount--;
        }

        public virtual void AddHeader(string instanceName, string header)
        {
            var headerBase = CreateListHeader(instanceName, header);
            headerBase.Top = AbsoluteContentHeight + _headerTopMargin;
            _listElement.AddChild(headerBase);
            _headerCount++;
            ResizeListElement();
        }

        public virtual void RemoveHeader(string instanceName)
        {
            DestroyListHeader(instanceName);
            _headerCount--;
            ResizeListElement();
        }

        private OverlayElementContainer CreateListElement(int width, int height)
        {
            var listElement = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/List", null, InstanceName);
            listElement.Width = width;
            listElement.Height = height;
            return listElement;
        }

        private void ResizeListElement()
        {
            _listElement.Width = _listWidth;
            _listElement.Height = AbsoluteContentHeight;
        }

        private OverlayElementContainer CreateListItem(string instanceName, string type, string header, string description, string iconCategory, string iconName, string iconLabel)
        {
            var listItemName = InstanceName + "/Item/" + instanceName;
            var listItemBase = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/ListItem", null, listItemName);
            listItemBase.GetChild(listItemName + "/ListItemHeader").Text = header;
            listItemBase.GetChild(listItemName + "/ListItemDescription").Text = description;
            listItemBase.Top = AbsoluteContentHeight;
            listItemBase.Width = _listWidth;
            var iconBase = CreateIcon(listItemName, type, iconCategory, iconName, iconLabel);
            iconBase.Top = 3;
            iconBase.Left = 3;
            ((OverlayElementContainer)listItemBase.GetChild(listItemName + "/ListItemContent")).Width = listItemBase.Width - 4;
            listItemBase.AddChildElement(iconBase);
            return listItemBase;
        }

        private void DestroyListItem(string instanceName)
        {
            var listItemName = InstanceName + "/Item/" + instanceName;
            var listItem = (OverlayElementContainer)_listElement.GetChild(listItemName);
            var iconInstanceName = listItemName + "/Icon";

            Globals.UI.RemoveButton(listItem);
            _listElement.RemoveChild(listItemName);
            listItem.RemoveChild(iconInstanceName);

            var iconInstance = OverlayManager.Instance.Elements.GetElement(iconInstanceName);
            if (iconInstance.SourceTemplate.Name.Contains("Portrait"))
                Globals.UI.DestroyPortrait(iconInstanceName);
            else if (iconInstance.SourceTemplate.Name.Contains("Icon"))
                Globals.UI.DestroyIcon(iconInstanceName);

            OverlayManager.Instance.Elements.DestroyElement(listItemName + "/ListItemContent");
            OverlayManager.Instance.Elements.DestroyElement(listItemName + "/ListItemHeader");
            OverlayManager.Instance.Elements.DestroyElement(listItemName + "/ListItemDescription");
            OverlayManager.Instance.Elements.DestroyElement(listItemName);
        }

        private OverlayElementContainer CreateIcon(string listItemName, string type, string category, string name, string label)
        {
            var instanceName = listItemName + "/Icon";
            OverlayElementContainer icon = null;

            if (type.Equals("Portrait"))
                icon = Globals.UI.CreatePortrait(instanceName, category, name, label);
            else if (type.Equals("Material"))
                icon = Globals.UI.CreateIcon(instanceName, category, name, label);

            return icon;
        }

        private OverlayElementContainer CreateListHeader(string instanceName, string header)
        {
            var listHeaderName = InstanceName + "/Header/" + instanceName;
            var listHeaderBase = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/ListHeader", null, listHeaderName);
            listHeaderBase.GetChild(listHeaderName + "/ListHeaderText").Text = header;
            return listHeaderBase;
        }

        private void DestroyListHeader(string instanceName)
        {
            var listHeaderName = InstanceName + "/Header/" + instanceName;
            _listElement.RemoveChild(listHeaderName);
            OverlayManager.Instance.Elements.DestroyElement(listHeaderName + "/ListHeaderText");
            OverlayManager.Instance.Elements.DestroyElement(listHeaderName);
        }
    }
}
