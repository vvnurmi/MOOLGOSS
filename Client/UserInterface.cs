using Axiom.Overlays;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private HashSet<OverlayElement> _buttons = new HashSet<OverlayElement>();
        private UIMode _mode;
        private int _mouseHideX;
        private int _mouseHideY;

        private bool IsInitialized { get { return Globals.UI != null; } }
        private Overlay Cursor { get { return OverlayManager.Instance.GetByName("Overlays/Cursor"); } }
        private OverlayElementContainer CursorPanel { get { return Cursor.GetChild("Overlays/Elements/CursorPanel"); } }
        private Overlay Dialog { get { return OverlayManager.Instance.GetByName("Overlays/Dialog"); } }
        private OverlayElementContainer DialogPanel { get { return Dialog.GetChild("Overlays/Elements/Dialog"); } }
        private OverlayElement ButtonTemplate { get { return OverlayManager.Instance.Elements.GetElement("Overlays/Templates/DialogButton", true); } }
        private IEnumerable<OverlayElement> DialogButtons { get { return DialogPanel.Children.Where(c => c.Key.Contains("/DialogButtons/")).Select(c => c.Value); } }
        private Overlay TitleScreen { get { return OverlayManager.Instance.GetByName("Overlays/TitleScreen"); } }

        public string Mode { get { return _mode == null ? "<none>" : _mode.Name; } }
        public bool IsTitleScreenVisible { get { return TitleScreen.IsVisible; } }
        public bool IsMouseVisible { get { return Cursor.IsVisible; } }

        public UserInterface()
        {
            if (Globals.Input != null) HideMouse();
        }

        public void AddButton(OverlayElement button)
        {
            if (!(button.UserData is Action)) throw new ArgumentException("Button UserData must be an Action");
            Debug.Assert(!_buttons.Contains(button));
            _buttons.Add(button);
        }

        public void RemoveButton(OverlayElement button)
        {
            var success = _buttons.Remove(button);
            Debug.Assert(success, "Button wasn't registered");
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
                AddButton(button);
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
                RemoveButton(button);
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
            if (IsInitialized && input.IsMouseDownEvent(Axiom.Input.MouseButtons.Left))
            {
                var button = _buttons.FirstOrDefault(b => b.IsVisible && Contains(b, input.AbsoluteMouseX, input.AbsoluteMouseY));
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

        public OverlayElementContainer CreateIcon(string instanceName, string category, string name, string count)
        {
            var iconBase = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/IconBase", null, instanceName);
            var iconImage = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/Icons/" + category + "/" + name, null, instanceName + "/IconImage");
            var iconOverlay = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/IconOverlay", null, instanceName + "/IconOverlay");
            iconOverlay.GetChild(instanceName + "/IconOverlay/IconText").Text = "" + count;

            iconBase.AddChildElement(iconImage);
            iconBase.AddChildElement(iconOverlay);
            iconBase.Top = 3;
            iconBase.Left = 3;

            return iconBase;
        }

        public void DestroyIcon(string instanceName)
        {
            OverlayManager.Instance.Elements.DestroyElement(instanceName + "/IconImage");
            OverlayManager.Instance.Elements.DestroyElement(instanceName + "/IconOverlay");
            OverlayManager.Instance.Elements.DestroyElement(instanceName + "/IconOverlay/IconText");
            OverlayManager.Instance.Elements.DestroyElement(instanceName);
        }

        public OverlayElementContainer CreateDarkButton(string instanceName, string label, int width, int height, int textSize)
        {
            var darkButtonBase = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/DarkButton", null, instanceName);
            darkButtonBase.GetChild(instanceName + "/DarkButtonText").Text = label;
            darkButtonBase.GetChild(instanceName + "/DarkButtonText").SetParam("char_height", "" + textSize);
            darkButtonBase.Height = height;
            darkButtonBase.Width = width;
            return darkButtonBase;
        }

        public void DestroyDarkButton(string instanceName)
        {
            OverlayManager.Instance.Elements.DestroyElement(instanceName + "/DarkButtonText");
            OverlayManager.Instance.Elements.DestroyElement(instanceName);
        }
    }
}
