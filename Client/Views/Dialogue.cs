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
        private OverlayElementContainer _dialogueElement;
        private string _name;
        private int _optionCount = 0;
        private int _optionVerticalMargin = 2;
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

        public void Destroy()
        {
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/DialogueImage");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/DialogueContent/BorderBL");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/DialogueContent/BorderBR");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/DialogueContent");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/TechTL");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/TechBL");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName);
            _dialogueElement = null;
        }

        public void RemoveOption(string name)
        {
            var dialogueContent = (OverlayElementContainer)_dialogueElement.GetChild(InstanceName + "/DialogueContent");
            var optionName = InstanceName + "/Option/" + name;
            Globals.UI.RemoveButton(dialogueContent.GetChild(optionName));
            dialogueContent.RemoveChild(optionName);
            Globals.UI.DestroyDarkButton(optionName);
            _optionCount--;
        }

        public void AddOption(string name, string content, Action action)
        {
            var option = CreateDialogueButton(InstanceName + "/Option/" + name, content);
            var dialogueContent = (OverlayElementContainer)_dialogueElement.GetChild(InstanceName + "/DialogueContent");
            dialogueContent.AddChildElement(option);
            option.Left = 6;
            option.VerticalAlignment = VerticalAlignment.Bottom;
            option.Top = -(((option.Height + _optionVerticalMargin) * (_optionCount + 1) + 4));
            option.UserData = action;
            Globals.UI.AddButton(option);
            _optionCount++;
        }

        private OverlayElementContainer CreateDialogueButton(string instanceName, string label)
        {
            var button = Globals.UI.CreateDarkButton(instanceName, label, _dialogueWidth - 20, 30, 16);
            return button;
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
    }
}