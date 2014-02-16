using Axiom.Overlays;

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
        private const string ButtonBarButtonBaseName = "Overlays/Elements/TopBarButtonInstance";
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

        public void AddButton(string name, string label)
        {
            var topBarButtonBarElement = (OverlayElementContainer)_topBarElement.GetChild(InstanceName + "/ButtonContainer");
            var button = CreateButton(name, label);
            button.Left = _buttonCount * (ButtonTemplate.Width + _buttonBarButtonMargin);
            _buttonCount++;
            topBarButtonBarElement.AddChild(button);
            ResizeButtonContainer();
        }

        public void RemoveButton(string name)
        {
            var topBarButtonBarElement = (OverlayElementContainer)_topBarElement.GetChild(InstanceName + "/ButtonContainer");
            topBarButtonBarElement.RemoveChild(ButtonBarButtonBaseName + "/" + _name + "_" + name);
            _buttonCount--;
            ResizeButtonContainer();
        }

        private OverlayElementContainer CreateButton(string name, string label)
        {
            var buttonName = ButtonBarButtonBaseName + "/" + _name + "_" + name;
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
