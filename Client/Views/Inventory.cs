using Axiom.Overlays;
using Core.Items;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryModel = Core.Items.Inventory;

namespace Client.Views
{
    /// <summary>
    /// Visualizes a <see cref="Core.Items.Inventory"/>.
    /// </summary>
    internal class Inventory
    {
        private string InstanceName { get { return "Overlays/Elements/Inventory/" + _name; } }
        private const string InventorySlotInstanceBaseName = "InventorySlot";
        private const string IconBaseName = "Overlays/Elements/IconInstance";
        private int _iconCount;
        private string _name;
        private int _slotCount;
        private Overlay _inventory;
        private InventoryModel _model;
        private int _modelChangeTimestamp;

        public bool IsVisible { get { return _inventory.IsVisible; } }

        public Inventory(string name, float x, float y, int slotCount, int slotCountX, InventoryModel model)
        {
            _name = name;
            _slotCount = slotCount;
            _inventory = OverlayManager.Instance.Create("Overlays/Inventory/" + _name);
            _model = model;
            _modelChangeTimestamp = -1;
            var inventoryElement = CreateOverlayElementContainer(x, y, slotCountX);
            _inventory.AddElement(inventoryElement);
        }

        public void SyncWithModel()
        {
            if (_modelChangeTimestamp == _model.ChangeTimestamp) return;
            _modelChangeTimestamp = _model.ChangeTimestamp;
            Clear();
            var slot = 0;
            foreach (var item in _model) AddItemStack(slot++, item);
        }

        public void Show()
        {
            _inventory.Show();
        }

        public void Hide()
        {
            _inventory.Hide();
        }

        private string GetSlotName(int slot)
        {
            return InstanceName + "/" + InventorySlotInstanceBaseName + slot;
        }

        private OverlayElementContainer GetSlot(int slot)
        {
            return (OverlayElementContainer)OverlayManager.Instance.Elements.GetElement(GetSlotName(slot));
        }

        private void AddItemStack(int slot, ItemStack stack)
        {
            var inventorySlot = GetSlot(slot);
            Debug.Assert(inventorySlot != null && !inventorySlot.Children.Any());
            var icon = CreateIcon(ItemTypes.GetCategoryName(stack.Type), ItemTypes.GetIconName(stack.Type), stack.Count);
            icon.UserData = new Action(() =>
            {
                var result = ItemTypes.Activate(stack.Type, Globals.PlayerShip.Pos);
                switch (result)
                {
                    case ItemActivationResult.IsDepleted:
                        // TODO: Only use one item off the stack.
                        // TODO: What if the item has been moved to another slot?
                        ClearSlot(slot);
                        break;
                    default: throw new NotImplementedException("Activation result " + result);
                }
            });
            Globals.UI.AddButton(icon);
            inventorySlot.AddChildElement(icon);
        }

        private void ClearSlot(int slot)
        {
            var inventorySlot = GetSlot(slot);
            if (inventorySlot == null || !inventorySlot.Children.Any()) return;
            var icon = inventorySlot.Children.First().Value;
            Globals.UI.RemoveButton(icon);
            inventorySlot.RemoveChild(icon.Name);
        }

        private void Clear()
        {
            for (int i = 0; i < _slotCount; i++) ClearSlot(i);
        }

        private OverlayElementContainer CreateIcon(string category, string name, int count)
        {
            var iconName = IconBaseName + "/" + _name + "_" + _iconCount;
            var iconBase = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/IconBase", null, iconName);
            var iconImage = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/Icons/" + category + "/" + name, null, iconName + "/IconImage");
            var iconOverlay = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/IconOverlay", null, iconName + "/IconOverlay");
            iconOverlay.GetChild(iconName + "/IconOverlay/IconText").Text = "" + count;

            iconBase.AddChildElement(iconImage);
            iconBase.AddChildElement(iconOverlay);
            iconBase.Top = 3;
            iconBase.Left = 3;

            _iconCount++;
            return iconBase;
        }

        private OverlayElementContainer CreateOverlayElementContainer(float x, float y, int slotCountX)
        {
            var inventory = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/InventoryBase", null, InstanceName);
            inventory.Left = x;
            inventory.Top = y;

            var inventorySlotMargin = 2;
            var inventorySlotTemplate = OverlayManager.Instance.Elements.GetElement("Overlays/Templates/WindowItemSlot", true);
            var inventoryBaseBorders = inventory.GetChild(InstanceName + "/InventoryBaseBorder");
            inventoryBaseBorders.Width = slotCountX * (inventorySlotTemplate.Width + inventorySlotMargin) + 14;
            var rowCount = (_slotCount + slotCountX - 1) / slotCountX;
            inventoryBaseBorders.Height = rowCount * (inventorySlotTemplate.Height + inventorySlotMargin) + 14;
            var inventoryContent = (OverlayElementContainer)inventory.GetChild(InstanceName + "/InventoryContent");
            inventoryContent.Width = inventoryBaseBorders.Width - 8;
            inventoryContent.Height = inventoryBaseBorders.Height - 8;

            for (int i = 0; i < _slotCount; i++)
            {
                int yPos = i / slotCountX;
                int xPos = i % slotCountX;
                var inventorySlot = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/WindowItemSlot", null, GetSlotName(i));
                inventorySlot.Left = xPos * (inventorySlot.Width + inventorySlotMargin) + 4;
                inventorySlot.Top = yPos * (inventorySlot.Height + inventorySlotMargin) + 4;
                inventoryContent.AddChildElement(inventorySlot);
            }

            return inventory;
        }
    }
}
