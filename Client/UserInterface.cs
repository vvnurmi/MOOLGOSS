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

    public class UIMode
    {
        public delegate void UpdateDelegate(float secondsPassed);

        public string Name { get; private set; }
        public Action Enter { get; set; }
        public UpdateDelegate Update { get; set; }
        public Action Exit { get; set; }

        public UIMode(string name)
        {
            Name = name;
            Enter = Exit = () => { };
            Update = secondsPassed => { };
        }
    }

    internal class UserInterface
    {
        private Dictionary<string, UIMode> _modes = new Dictionary<string, UIMode>();
        private UIMode _mode;
        private int _mouseHideX;
        private int _mouseHideY;

        private bool IsInitialized { get { return Globals.UI != null; } }
        private string InventoryInstanceBaseName = "Overlays/Elements/InventoryInstance";
        private string InventorySlotInstanceBaseName = "InventorySlot";
        private string IconBaseName = "Overlays/Elements/IconInstance";
        private int IconCount = 0;
        private string PlayerInventoryName = "Player0";
        private Overlay PlayerInventory { get { return OverlayManager.Instance.GetByName("Overlays/PlayerInventory"); } }
        private OverlayElementContainer PlayerInventoryElement = null;
        private Overlay TestWindow { get { return OverlayManager.Instance.GetByName("Overlays/TestWindow"); } }
        private Overlay Cursor { get { return OverlayManager.Instance.GetByName("Overlays/Cursor"); } }
        private OverlayElementContainer CursorPanel { get { return Cursor.GetChild("Overlays/Elements/CursorPanel"); } }
        private Overlay Dialog { get { return OverlayManager.Instance.GetByName("Overlays/Dialog"); } }
        private OverlayElementContainer DialogPanel { get { return Dialog.GetChild("Overlays/Elements/Dialog"); } }
        private OverlayElement ButtonTemplate { get { return OverlayManager.Instance.Elements.GetElement("Overlays/Templates/DialogButton", true); } }
        private IEnumerable<OverlayElement> DialogButtons { get { return DialogPanel.Children.Where(c => c.Key.Contains("/DialogButtons/")).Select(c => c.Value); } }
        private Overlay TitleScreen { get { return OverlayManager.Instance.GetByName("Overlays/TitleScreen"); } }

        public string Mode { get { return _mode == null ? "<none>" : _mode.Name; } }
        public bool IsPlayerInventoryVisible { get { return PlayerInventory.IsVisible; } }
        public bool IsTitleScreenVisible { get { return TitleScreen.IsVisible; } }
        public bool IsMouseVisible { get { return Cursor.IsVisible; } }

        public UserInterface()
        {
            HideMouse();
        }

        public bool TryShowTestWindow()
        {
            if (TestWindow.IsVisible) return false;
            TestWindow.Show();
            return true;
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
            Globals.Input.SetMousePosition(_mouseHideX, _mouseHideY);
            CursorPanel.Left = _mouseHideX;
            CursorPanel.Top = _mouseHideY;
            Cursor.Show();
        }

        public void HideMouse()
        {
            _mouseHideX = (int)Math.Round(Globals.Input.AbsoluteMouseX);
            _mouseHideY = (int)Math.Round(Globals.Input.AbsoluteMouseY);
            Cursor.Hide();
        }

        public void Update(float secondsPassed)
        {
            var input = Globals.Input;
            if (IsInitialized && IsMouseVisible)
            {
                CursorPanel.Left = input.AbsoluteMouseX;
                CursorPanel.Top = input.AbsoluteMouseY;
            }
            if (_mode != null) _mode.Update(secondsPassed);
            if (IsInitialized)
            {
                if (!Dialog.IsVisible) return;
                if (!input.IsMouseDownEvent(Axiom.Input.MouseButtons.Left)) return;
                var button = DialogButtons.FirstOrDefault(b => Contains(b, input.AbsoluteMouseX, input.AbsoluteMouseY));
                if (button != null) ((Action)button.UserData)();
            }
        }

        public void AddMode(UIMode mode)
        {
            _modes.Add(mode.Name, mode);
        }

        public void SetMode(string name)
        {
            if (_mode != null) _mode.Exit();
            _mode = name == null ? null : _modes[name];
            if (_mode != null) _mode.Enter();
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

        public void ShowPlayerInventory()
        {
            if (PlayerInventoryElement == null)
            {
                PlayerInventoryElement = CreateInventory(PlayerInventoryName, 10, 10, 28, 5);
                PlayerInventory.AddElement(PlayerInventoryElement);
                AddItemToInventory(PlayerInventoryName, 3, "Solid", "IronOre", 23);
                AddItemToInventory(PlayerInventoryName, 10, "Data", "Default", 310);
                //RemoveItemFromInventory(PlayerInventoryName, 3);
            }
            
            if (!IsPlayerInventoryVisible)
                PlayerInventory.Show();
        }

        public void HidePlayerInventory()
        {
            if (IsPlayerInventoryVisible)
                PlayerInventory.Hide();
        }

        public void AddItemToInventory(string inventoryName, int slot, string iconCategory, string iconName, int count)
        {
            var inventorySlot = (OverlayElementContainer)OverlayManager.Instance.Elements.GetElement(InventoryInstanceBaseName + inventoryName + "/" + InventorySlotInstanceBaseName + slot);

            if (inventorySlot != null)
                inventorySlot.AddChildElement(CreateIcon(iconCategory, iconName, count));
        }

        public void RemoveItemFromInventory(string inventoryName, int slot)
        {
            var inventorySlot = (OverlayElementContainer)OverlayManager.Instance.Elements.GetElement(InventoryInstanceBaseName + inventoryName + "/" + InventorySlotInstanceBaseName + slot);
            if (inventorySlot != null && inventorySlot.Children.Count > 0)
                    inventorySlot.RemoveChild(inventorySlot.Children.ElementAt(0).Value.Name);
        }

        private OverlayElementContainer CreateIcon(string category, string name, int count)
        {
            var iconName = IconBaseName + "_" + IconCount;
            var iconBase = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/IconBase", null, iconName);
            var iconImage = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/Icons/" + category + "/" + name, null, iconName + "/IconImage");
            var iconOverlay = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/IconOverlay", null, iconName + "/IconOverlay");
            iconOverlay.GetChild(iconName + "/IconOverlay/Count").Text = "" + count;
            
            iconBase.AddChildElement(iconImage);
            iconBase.AddChildElement(iconOverlay);
            iconBase.Top = 3;
            iconBase.Left = 3;

            IconCount++;
            return iconBase;
        }

        private OverlayElementContainer CreateInventory(string name, float x, float y, int size, int width)
        {
            var inventoryInstanceName = InventoryInstanceBaseName + name;
            var inventory = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/InventoryBase", null, inventoryInstanceName);
            inventory.Left = x;
            inventory.Top = y;

            var inventorySlotMargin = 2;
            OverlayElement inventorySlotTemplate = (OverlayElement)OverlayManager.Instance.Elements.GetElement("Overlays/Templates/WindowItemSlot", true);
            var inventoryBaseBorders = (OverlayElementContainer)inventory.GetChild(inventoryInstanceName + "/InventoryBaseBorder");
            inventoryBaseBorders.Width = width * (inventorySlotTemplate.Width + inventorySlotMargin) + 14;
            inventoryBaseBorders.Height = ((int)Math.Ceiling((Decimal)size / (Decimal)width) * (inventorySlotTemplate.Height + inventorySlotMargin)) + 14;
            var inventoryContent = (OverlayElementContainer)inventory.GetChild(inventoryInstanceName + "/InventoryContent");
            inventoryContent.Width = inventoryBaseBorders.Width - 8;
            inventoryContent.Height = inventoryBaseBorders.Height - 8;

            for (int i = 0; i < size; i++)
            {
                int yPos = i / width;
                int xPos = i % width;
                var inventorySlotInstanceName = inventoryInstanceName + "/" + InventorySlotInstanceBaseName + i;
                var inventorySlot = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/WindowItemSlot", null, inventorySlotInstanceName);
                inventorySlot.Left = xPos * (inventorySlot.Width + inventorySlotMargin) + 4;
                inventorySlot.Top = yPos * (inventorySlot.Height + inventorySlotMargin) + 4;
                inventoryContent.AddChildElement(inventorySlot);
            }

            return inventory;
        }
    }
}
