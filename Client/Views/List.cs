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
        public OverlayElement ListItemTemplate { get { return OverlayManager.Instance.Elements.GetElement("Overlays/Templates/ListItem", true); } }
        private OverlayElementContainer _listElement;
        private string _name;
        private int _listItemCount = 0;
        private int _listItemVerticalMargin = 2;
        private int _listWidth = 0;
        private int _listHeight = 0;

        public OverlayElementContainer ListElement { get { return _listElement; } }
        public int ItemCount { get { return _listItemCount; } }

        public List(string name, int width, int height)
        {
            _name = name;
            _listWidth = width;
            //_listHeight = height;
            _listElement = CreateListElement(_listWidth, 50);
        }

        public virtual void Destroy()
        {
            /*
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/ListContent/BorderTL");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/ListContent/BorderTR");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/ListContent/BorderBL");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/ListContent/BorderBR");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/ListContent");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/TechTR");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/TechBR");
            */
            OverlayManager.Instance.Elements.DestroyElement(InstanceName);
        }

        public virtual void AddItem(string instanceName, string type, string header, string description, string iconCategory, string iconName, string iconLabel, Action action)
        {
            var listItemBase = CreateListItem(instanceName, type, header, description, iconCategory, iconName, iconLabel);
            //((OverlayElementContainer)_listElement.GetChild(InstanceName + "/ListContent")).AddChildElement(listItemBase);
            _listElement.AddChild(listItemBase);
            listItemBase.UserData = action;
            Globals.UI.AddButton(listItemBase);
            _listItemCount++;
            ResizeListElement();
        }

        public virtual void RemoveItem(string instanceName)
        {
            var listItemName = InstanceName + "/Item/" + instanceName;
            //var listContent = (OverlayElementContainer)_listElement.GetChild(InstanceName + "/ListContent");
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

            _listItemCount--;
        }

        private OverlayElementContainer CreateListElement(int width, int height)
        {
            var listElement = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/List", null, InstanceName);
            listElement.Width = width;
            listElement.Height = height;
            //((OverlayElementContainer)listElement.GetChild(InstanceName + "/ListContent")).Width = width - 8;
            //((OverlayElementContainer)listElement.GetChild(InstanceName + "/ListContent")).Height = height - 8;
            return listElement;
        }

        private void ResizeListElement()
        {
            _listElement.Width = _listWidth;
            _listElement.Height = _listItemCount * (ListItemTemplate.Height + _listItemVerticalMargin);
        }

        private OverlayElementContainer CreateListItem(string instanceName, string type, string header, string description, string iconCategory, string iconName, string iconLabel)
        {
            var listItemName = InstanceName + "/Item/" + instanceName;
            var listItemBase = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/ListItem", null, listItemName);
            listItemBase.GetChild(listItemName + "/ListItemHeader").Text = header;
            listItemBase.GetChild(listItemName + "/ListItemDescription").Text = description;

            var iconBase = CreateIcon(listItemName, type, iconCategory, iconName, iconLabel);
            iconBase.Top = 3;
            iconBase.Left = 3;
            //listItemBase.Left = 7;
            listItemBase.Top = _listItemCount * (listItemBase.Height + _listItemVerticalMargin);
            listItemBase.Width = _listWidth;
            ((OverlayElementContainer)listItemBase.GetChild(listItemName + "/ListItemContent")).Width = listItemBase.Width - 4;
            listItemBase.AddChildElement(iconBase);
            return listItemBase;
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
    }
}
