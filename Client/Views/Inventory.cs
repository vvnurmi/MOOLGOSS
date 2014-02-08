using Axiom.Overlays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Views
{
    /// <summary>
    /// Visualizes a <see cref="Core.Items.Inventory"/>.
    /// </summary>
    internal class Inventory
    {
        private string InventoryInstanceBaseName = "Overlays/Elements/InventoryInstance";
        private string InventorySlotInstanceBaseName = "InventorySlot";
        private string IconBaseName = "Overlays/Elements/IconInstance";
        private int IconCount = 0;
        private string _name;
        private Overlay _playerInventory;

        public bool IsVisible { get { return _playerInventory.IsVisible; } }

        public Inventory(string name, float x, float y, int size, int width)
        {
            _name = name;
            _playerInventory = OverlayManager.Instance.Create("Overlays/PlayerInventory");
            var playerInventoryElement = CreateInventory(x, y, size, width);
            _playerInventory.AddElement(playerInventoryElement);
        }

        public void Show()
        {
            _playerInventory.Show();
        }

        public void Hide()
        {
            _playerInventory.Hide();
        }

        public void AddItemToInventory(int slot, string iconCategory, string iconName, int count)
        {
            var inventorySlot = (OverlayElementContainer)OverlayManager.Instance.Elements.GetElement(InventoryInstanceBaseName + _name + "/" + InventorySlotInstanceBaseName + slot);

            if (inventorySlot != null)
                inventorySlot.AddChildElement(CreateIcon(iconCategory, iconName, count));
        }

        public void RemoveItemFromInventory(int slot)
        {
            var inventorySlot = (OverlayElementContainer)OverlayManager.Instance.Elements.GetElement(InventoryInstanceBaseName + _name + "/" + InventorySlotInstanceBaseName + slot);
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

        private OverlayElementContainer CreateInventory(float x, float y, int size, int width)
        {
            var inventoryInstanceName = InventoryInstanceBaseName + _name;
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
