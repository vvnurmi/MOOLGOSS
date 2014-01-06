using Axiom.Overlays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class UserInterface
    {
        private Overlay Dialog { get { return OverlayManager.Instance.GetByName("Overlays/Dialog"); } }
        private OverlayElementContainer DialogPanel { get { return Dialog.GetChild("Overlays/Elements/Dialog"); } }
        private OverlayElement ButtonTemplate { get { return OverlayManager.Instance.Elements.GetElement("Overlays/Templates/DialogButton", true); } }

        public bool TryShowDialog(string text, params string[] buttonTexts)
        {
            if (Dialog.IsVisible) return false;
            DialogPanel.GetChild("Overlays/Elements/DialogText").Text = text;
            var space = (DialogPanel.Width - buttonTexts.Length * ButtonTemplate.Width) / (buttonTexts.Length + 1);
            var buttonX = space;
            var buttonY = DialogPanel.Height - 2 * ButtonTemplate.Height;
            foreach (var buttonText in buttonTexts)
            {
                var button = CreateDialogButton(buttonText, buttonX, buttonY);
                DialogPanel.AddChild(button);
                buttonX += space + ButtonTemplate.Width;
            }
            Dialog.Show();
            return true;
        }

        public void HideDialog()
        {
            Dialog.Hide();
            foreach (var button in DialogPanel.Children.Where(c => c.Key.Contains("/DialogButtons/")).ToArray())
            {
                DialogPanel.RemoveChild(button.Key);
                OverlayManager.Instance.Elements.DestroyElement(button.Key);
            }
        }

        private OverlayElementContainer CreateDialogButton(string name, float x, float y)
        {
            var instanceName = "Overlays/Elements/DialogButtons/" + name;
            var button = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate(
                "Overlays/Templates/DialogButton", null, instanceName);
            button.GetChild(instanceName + "/Text").Text = name;
            button.Left = x;
            button.Top = y;
            return button;
        }
    }
}
