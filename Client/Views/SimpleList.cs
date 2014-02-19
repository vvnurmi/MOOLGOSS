using Axiom.Overlays;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Views
{
    class SimpleList
    {
        private string InstanceName { get { return "Overlays/Elements/SimpleList/" + _name; } }
        private OverlayElementContainer _simpleListElement;
        private string _name;
        private int _listItemCount = 0;
        private int _listItemVerticalMargin = 2;
        private int _listWidth = 0;
        private int _listHeight = 0;

        public OverlayElementContainer ListElement { get { return _simpleListElement; } }

        public SimpleList(string name, int width, int height)
        {
            _name = name;
            _listWidth = width;
            _listHeight = height;
            _simpleListElement = CreateSimpleListElement(_listWidth, _listHeight);
        }

        public void AddItem(string instanceName, string header, string description, string iconCategory, string iconName, string iconLabel, Action action)
        {
            var listItemBase = CreateListItem(instanceName, header, description, iconCategory, iconName, iconLabel);
            ((OverlayElementContainer)_simpleListElement.GetChild(InstanceName + "/SimpleListContent")).AddChildElement(listItemBase);
            listItemBase.UserData = action;
            Globals.UI.AddButton(listItemBase);
            _listItemCount++;
        }

        public void RemoveItem(string instanceName)
        {
            var listItemName = InstanceName + "/Item/" + instanceName;
            var listContent = (OverlayElementContainer)_simpleListElement.GetChild(InstanceName + "/SimpleListContent");
            var listItem = (OverlayElementContainer)listContent.GetChild(listItemName);
            var iconInstanceName = listItemName + "/Icon";

            Globals.UI.RemoveButton(listItem);
            listContent.RemoveChild(listItemName);

            listItem.RemoveChild(iconInstanceName);
            Globals.UI.DestroyIcon(iconInstanceName);

            OverlayManager.Instance.Elements.DestroyElement(listItemName + "/SimpleListItemContent");
            OverlayManager.Instance.Elements.DestroyElement(listItemName + "/SimpleListItemHeader");
            OverlayManager.Instance.Elements.DestroyElement(listItemName + "/SimpleListItemDescription");
            OverlayManager.Instance.Elements.DestroyElement(listItemName);

            _listItemCount--;
        }

        private OverlayElementContainer CreateSimpleListElement(int width, int height)
        {
            var simpleListElement = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/SimpleList", null, InstanceName);
            simpleListElement.Width = width;
            simpleListElement.Height = height;
            ((OverlayElementContainer)simpleListElement.GetChild(InstanceName + "/SimpleListContent")).Width = width - 8;
            ((OverlayElementContainer)simpleListElement.GetChild(InstanceName + "/SimpleListContent")).Height = height - 8;
            return simpleListElement;
        }

        private OverlayElementContainer CreateListItem(string instanceName, string header, string description, string iconCategory, string iconName, string iconLabel)
        {
            var listItemName = InstanceName + "/Item/" + instanceName;
            var listItemBase = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/SimpleListItem", null, listItemName);
            listItemBase.GetChild(listItemName + "/SimpleListItemHeader").Text = header;
            listItemBase.GetChild(listItemName + "/SimpleListItemDescription").Text = description;

            var iconBase = CreateIcon(listItemName, iconCategory, iconName, iconLabel);
            iconBase.Top = 3;
            iconBase.Left = 3;
            listItemBase.Left = 7;
            listItemBase.Top = _listItemCount * (listItemBase.Height + _listItemVerticalMargin) + 7;
            listItemBase.Width = _listWidth - 22;
            ((OverlayElementContainer)listItemBase.GetChild(listItemName + "/SimpleListItemContent")).Width = listItemBase.Width - 4;
            listItemBase.AddChildElement(iconBase);
            return listItemBase;
        }

        private OverlayElementContainer CreateIcon(string listItemName, string category, string name, string label)
        {
            var instanceName = listItemName + "/Icon";
            return Globals.UI.CreateIcon(instanceName, category, name, label);
        }
    }
}
