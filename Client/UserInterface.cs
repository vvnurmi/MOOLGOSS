using Axiom.Overlays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal struct ButtonDef
    {
        public string Name;
        public Action Pressed;
    }

    internal class UserInterface
    {
        private Overlay Inventory { get { return OverlayManager.Instance.GetByName("Overlays/Inventory"); } }
        private Overlay TestWindow { get { return OverlayManager.Instance.GetByName("Overlays/TestWindow"); } }
        private Overlay Cursor { get { return OverlayManager.Instance.GetByName("Overlays/Cursor"); } }
        private OverlayElementContainer CursorPanel { get { return Cursor.GetChild("Overlays/Elements/CursorPanel"); } }
        private Overlay Dialog { get { return OverlayManager.Instance.GetByName("Overlays/Dialog"); } }
        private OverlayElementContainer DialogPanel { get { return Dialog.GetChild("Overlays/Elements/Dialog"); } }
        private OverlayElement ButtonTemplate { get { return OverlayManager.Instance.Elements.GetElement("Overlays/Templates/DialogButton", true); } }
        private IEnumerable<OverlayElement> DialogButtons { get { return DialogPanel.Children.Where(c => c.Key.Contains("/DialogButtons/")).Select(c => c.Value); } }
        private Overlay TitleScreen { get { return OverlayManager.Instance.GetByName("Overlays/TitleScreen"); } }

        public bool IsInventoryVisible { get { return Inventory.IsVisible; } }
        public bool IsTitleScreenVisible { get { return TitleScreen.IsVisible; } }
        public bool IsMouseVisible { get { return Cursor.IsVisible; } }

        public bool TryShowTestWindow()
        {
            if (TestWindow.IsVisible) return false;
            TestWindow.Show();
            return true;
        }

        public void ShowInventory()
        {
            Inventory.Show();
        }

        public void HideInventory()
        {
            Inventory.Hide();
        }

        public bool TryShowDialog(string text, params ButtonDef[] buttonDefs)
        {
            if (Dialog.IsVisible) return false;
            DialogPanel.GetChild("Overlays/Elements/DialogText").Text = text;
            var space = (DialogPanel.Width - buttonDefs.Length * ButtonTemplate.Width) / (buttonDefs.Length + 1);
            var buttonX = space;
            var buttonY = DialogPanel.Height - 2 * ButtonTemplate.Height;
            foreach (var def in buttonDefs)
            {
                var button = CreateDialogButton(def.Name, buttonX, buttonY);
                button.UserData = def.Pressed;
                DialogPanel.AddChild(button);
                buttonX += space + ButtonTemplate.Width;
            }
            Dialog.Show();
            return true;
        }

        public void HideDialog()
        {
            Dialog.Hide();
            foreach (var button in DialogButtons.ToArray())
            {
                DialogPanel.RemoveChild(button.Name);
                OverlayManager.Instance.Elements.DestroyElement(button.Name);
            }
        }

        public void ShowTitleScreen()
        {
            TitleScreen.Show();
        }

        public void HideTitleScreen()
        {
            TitleScreen.Hide();
        }

        public void ShowMouse()
        {
            CursorPanel.Left = Globals.Input.AbsoluteMouseX;
            CursorPanel.Top = Globals.Input.AbsoluteMouseY;
            Cursor.Show();
        }

        public void HideMouse()
        {
            Cursor.Hide();
        }

        public void Update()
        {
            var input = Globals.Input;
            if (IsMouseVisible)
            {
                CursorPanel.Left = input.AbsoluteMouseX;
                CursorPanel.Top = input.AbsoluteMouseY;
            }
            if (!Dialog.IsVisible) return;
            if (!input.IsMouseDownEvent(Axiom.Input.MouseButtons.Left)) return;
            var button = DialogButtons.FirstOrDefault(b => Contains(b, input.AbsoluteMouseX, input.AbsoluteMouseY));
            if (button == null) return;
            ((Action)button.UserData)();
        }

        private static bool Contains(OverlayElement e, float x, float y)
        {
            var actualLeftPx = Globals.Scene.CurrentViewport.ActualWidth * e.DerivedLeft;
            var actualTopPx = Globals.Scene.CurrentViewport.ActualHeight * e.DerivedTop;
            return
                actualLeftPx <= x &&
                actualLeftPx + e.Width > x &&
                actualTopPx <= y &&
                actualTopPx + e.Height > y;
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
