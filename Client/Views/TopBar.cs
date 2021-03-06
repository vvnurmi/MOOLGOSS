﻿using Axiom.Overlays;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Views
{
    class TopBar
    {
        private string InstanceName { get { return "Overlays/Elements/TopBar/" + _name; } }
        private OverlayElement ButtonTemplate { get { return OverlayManager.Instance.Elements.GetElement("Overlays/Templates/SimpleDarkButton", true); } }
        private OverlayElementContainer _topBarElement;
        private Overlay _topBar;
        private string _name;
        private string _defaultLocation;
        private int _buttonCount;
        private int _buttonBarButtonMargin = 0;

        public bool IsVisible { get { return _topBar.IsVisible; } }

        public TopBar(string name, string defaultLocation)
        {
            _name = name;
            _defaultLocation = defaultLocation;
            _topBar = OverlayManager.Instance.Create("Overlays/TopBar/" + _name);
            _topBarElement = CreateTopBarElement();
            SetLocationText(_defaultLocation);
            _topBar.AddElement(_topBarElement);
        }

        private OverlayElementContainer CreateTopBarElement()
        {
            var topBarElement = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/TopBar", null, InstanceName);
            return topBarElement;
        }

        public void SetLocationText(string location)
        {
            _topBarElement.GetChild(InstanceName + "/LocationText").Text = location;
        }

        public void AddButton(string name, string label, Action action)
        {
            var topBarButtonBarElement = (OverlayElementContainer)_topBarElement.GetChild(InstanceName + "/ButtonContainer");
            var button = CreateButton(name, label);
            button.Left = _buttonCount * (ButtonTemplate.Width + _buttonBarButtonMargin);
            button.UserData = action;
            _buttonCount++;
            Globals.UI.AddButton(button);
            topBarButtonBarElement.AddChild(button);
            ResizeButtonContainer();
        }

        public void RemoveButton(string name)
        {
            var topBarButtonBarElement = (OverlayElementContainer)_topBarElement.GetChild(InstanceName + "/ButtonContainer");
            var buttonInstanceName = InstanceName + "/Button/" + name;
            var button = (OverlayElementContainer)topBarButtonBarElement.GetChild(buttonInstanceName);
            Globals.UI.RemoveButton(button);
            topBarButtonBarElement.RemoveChild(buttonInstanceName);

            OverlayManager.Instance.Elements.DestroyElement(buttonInstanceName + "/SimpleDarkButtonContent");
            OverlayManager.Instance.Elements.DestroyElement(buttonInstanceName + "/SimpleDarkButtonText");
            OverlayManager.Instance.Elements.DestroyElement(buttonInstanceName);

            _buttonCount--;
            ResizeButtonContainer();
        }

        private OverlayElementContainer CreateButton(string name, string label)
        {
            var buttonName = InstanceName + "/Button/" + name;
            var buttonBase = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/SimpleDarkButton", null, buttonName);
            buttonBase.GetChild(buttonName + "/SimpleDarkButtonText").Text = label;

            return buttonBase;
        }

        private void ResizeButtonContainer()
        {
            var topBarHotBarElement = (OverlayElementContainer)_topBarElement.GetChild(InstanceName + "/ButtonContainer");
            topBarHotBarElement.Width = _buttonCount * (ButtonTemplate.Width + _buttonBarButtonMargin);
            topBarHotBarElement.Left = -(float)Math.Round(topBarHotBarElement.Width / 2);
        }

        public void Show()
        {
            _topBar.Show();
        }

        public void Hide()
        {
            _topBar.Hide();
        }
    }
}
