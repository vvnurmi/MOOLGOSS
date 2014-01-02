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

        public bool TryShowDialog(string text)
        {
            if (Dialog.IsVisible) return false;
            Dialog
                .GetChild("Overlays/Elements/Dialog")
                .GetChild("Overlays/Elements/DialogText").Text = text;
            Dialog.Show();
            return true;
        }

        public void HideDialog()
        {
            Dialog.Hide();
        }
    }
}
