using Axiom.Overlays;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Views
{
    class Dialogue
    {
        private string InstanceName { get { return "Overlays/Elements/Dialogue/" + _name; } }
        private OverlayElementContainer DialogueContent { get { return (OverlayElementContainer)_dialogueElement.GetChild(InstanceName + "/DialogueContent"); } }
        private OverlayElementContainer DialogueImage { get { return (OverlayElementContainer)_dialogueElement.GetChild(InstanceName + "/DialogueImage"); } }
        private OverlayElement ButtonBlueTemplate { get { return OverlayManager.Instance.Elements.GetElement("Overlays/Templates/WindowButtonBlue", true); } }
        private OverlayElementContainer _dialogueElement;
        private string _name;
        private string _portrait;
        private int _optionCount = 0;
        private int _optionVerticalMargin = 2;
        private int _buttonCount = 0;
        private int _buttonVerticalMargin = 2;
        private int _propertyCount = 0;
        private int _propertyVerticalMargin = 2;
        private int _dialogueWidth = 0;
        private int _dialogueHeight = 0;

        public OverlayElementContainer DialogueElement { get { return _dialogueElement; } }

        public Dialogue(string name, int width, int height)
        {
            _name = name;
            _dialogueWidth = width;
            _dialogueHeight = height;
            _dialogueElement = CreateDialogueElement();
        }

        private OverlayElementContainer CreateDialogueElement()
        {
            var dialogueElement = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/Dialogue", null, InstanceName);
            dialogueElement.Width = _dialogueWidth;
            dialogueElement.Height = _dialogueHeight;
            var dialogueImage = (OverlayElementContainer)dialogueElement.GetChild(InstanceName + "/DialogueImage");
            dialogueImage.Width = _dialogueWidth - 8;
            var dialogueContent = (OverlayElementContainer)dialogueElement.GetChild(InstanceName + "/DialogueContent");
            dialogueContent.Width = _dialogueWidth - 8;
            dialogueContent.Height = _dialogueHeight - dialogueImage.Height - 8;
            return dialogueElement;
        }

        public void Destroy()
        {
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/DialogueImage");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/DialogueContent/BorderBL");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/DialogueContent/BorderBR");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/DialogueContent");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/TechTL");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/TechBL");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName);
            RemovePortrait();
            _dialogueElement = null;
        }

        public void AddButton(string name, string type, string content, Action action)
        {
            var button = CreateButton(InstanceName + "/Button/" + name, type, content);
            DialogueContent.AddChildElement(button);
            button.Left = 6;
            button.VerticalAlignment = VerticalAlignment.Bottom;
            button.Top = -((button.Height + _buttonVerticalMargin) * (_buttonCount + 1) + 4);
            button.UserData = action;
            Globals.UI.AddButton(button);
            _buttonCount++;
        }

        private OverlayElementContainer CreateButton(string instanceName, string type, string label)
        {
            var button = Globals.UI.CreateButton(instanceName, type, label, _dialogueWidth - 20, 30, 18);
            return button;
        }

        public void RemoveButton(string name)
        {
            var dialogueContent = (OverlayElementContainer)_dialogueElement.GetChild(InstanceName + "/DialogueContent");
            var buttonName = InstanceName + "/Button/" + name;
            Globals.UI.RemoveButton(dialogueContent.GetChild(buttonName));
            dialogueContent.RemoveChild(buttonName);
            Globals.UI.DestroyButton(buttonName);
            _buttonCount--;
        }

        public void AddOption(string name, string content, Action action)
        {
            var option = CreateOptionButton(InstanceName + "/Option/" + name, content);
            DialogueContent.AddChildElement(option);
            option.Left = 6;
            option.VerticalAlignment = VerticalAlignment.Bottom;
            var buttonsHeight = (ButtonBlueTemplate.Height + _buttonVerticalMargin) * _buttonCount;
            if (_buttonCount > 0) buttonsHeight += 25;
            option.Top = -((option.Height + _optionVerticalMargin) * (_optionCount + 1) + buttonsHeight + 4);
            option.UserData = action;
            Globals.UI.AddButton(option);
            _optionCount++;
        }

        private OverlayElementContainer CreateOptionButton(string instanceName, string label)
        {
            var button = Globals.UI.CreateDarkButton(instanceName, label, _dialogueWidth - 20, 30, 16);
            return button;
        }

        public void RemoveOption(string name)
        {
            var optionName = InstanceName + "/Option/" + name;
            Globals.UI.RemoveButton(DialogueContent.GetChild(optionName));
            DialogueContent.RemoveChild(optionName);
            Globals.UI.DestroyDarkButton(optionName);
            _optionCount--;
        }

        public void AddProperty(string name, string header, string content, Action action)
        {
            var property = CreateProperty(InstanceName + "/Property/" + name, header, content);
            DialogueContent.AddChildElement(property);
            property.VerticalAlignment = VerticalAlignment.Top;
            property.Left = 6;
            property.Top = (property.Height + _propertyVerticalMargin) * _propertyCount + 4;
            _propertyCount++;
        }

        private OverlayElementContainer CreateProperty(string instanceName, string header, string content)
        {
            var property = Globals.UI.CreateProperty(instanceName, header, content, _dialogueWidth - 20, 20, 85, 17);
            return property;
        }

        public void RemoveProperty(string name)
        {
            var propertyName = InstanceName + "/Property/" + name;
            DialogueContent.RemoveChild(propertyName);
            Globals.UI.DestroyProperty(propertyName);
            _optionCount--;
        }

        public void SetPortrait(string name)
        {
            if (_portrait != name)
            {
                _portrait = name;
                var portrait = CreatePortrait(InstanceName + "/Portrait", name);
                DialogueImage.AddChildElement(portrait);
            }
        }

        public OverlayElementContainer CreatePortrait(string instanceName, string name)
        {
            var portrait = Globals.UI.CreatePortrait(instanceName, "Large", name, null);
            return portrait;
        }

        public void RemovePortrait()
        {
            if (_portrait != null)
            {
                var portraitName = InstanceName + "/Portrait";
                DialogueImage.RemoveChild(portraitName);
                Globals.UI.DestroyPortrait(portraitName);
                _portrait = null;
            }
        }
    }
}