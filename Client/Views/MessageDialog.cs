using Axiom.Overlays;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Views
{
    class MessageDialog
    {
        private string InstanceName { get { return "Overlays/Elements/MessageDialog/" + _name; } }

        private string _name;
        private string _message;
        private int _dialogWidth;

        private Overlay _dialog;
        private OverlayElementContainer _dialogElement;
        private OverlayElementContainer _cancelButton;
        private OverlayElementContainer _confirmButton;

        public bool IsVisible { get { return _dialog.IsVisible; } }
        
        public MessageDialog(string name, int width)
        {
            _name = name;
            _dialogWidth = width;
            _dialog = OverlayManager.Instance.Create("Overlays/MessageDialog/" + _name);
            _dialogElement = CreateDialogElement();
            _dialog.AddElement(_dialogElement);
            ResizeElement();
        }

        public void Destroy()
        {
            var dialogContent = (OverlayElementContainer)OverlayManager.Instance.Elements.GetElement(InstanceName + "/MessageDialogContent");
            if (_confirmButton != null)
            {
                Globals.UI.RemoveButton(_confirmButton);
                Globals.UI.DestroyButton(_confirmButton.Name);
                dialogContent.RemoveChild(_confirmButton.Name);
            }
            if (_cancelButton != null)
            {
                Globals.UI.RemoveButton(_cancelButton);
                Globals.UI.DestroyButton(_cancelButton.Name);
                dialogContent.RemoveChild(_cancelButton.Name);
            }

            _dialog.RemoveElement(_dialogElement);
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/MessageDialogContent/MessageDialogMessage");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/MessageDialogContent");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName + "/MessageDialogBorder");
            OverlayManager.Instance.Elements.DestroyElement(InstanceName);
            OverlayManager.Instance.Destroy(_dialog);
        }

        public void ResizeElement()
        {
            var lines = 0;
            var height = 0;

            if (_message != null)
                lines = _message.Count(c => c == '\n') + 1;

            height += lines * 17 + 19;

            if (_cancelButton != null || _confirmButton != null)
                height += 30;

            _dialogElement.Width = _dialogWidth;
            _dialogElement.Height = height;

            var dialogContent = (OverlayElementContainer)OverlayManager.Instance.Elements.GetElement(InstanceName + "/MessageDialogContent");
            dialogContent.Width = _dialogElement.Width - (dialogContent.Left * 2);
            dialogContent.Height = _dialogElement.Height - (dialogContent.Top * 2);

            var dialogBorder = (OverlayElementContainer)OverlayManager.Instance.Elements.GetElement(InstanceName + "/MessageDialogBorder");
            dialogBorder.Width = _dialogElement.Width;
            dialogBorder.Height = _dialogElement.Height;

            _dialogElement.Left = Globals.Scene.CurrentViewport.ActualWidth / 2 - _dialogElement.Width / 2;
            _dialogElement.Top = Globals.Scene.CurrentViewport.ActualHeight / 2 - _dialogElement.Height / 2;

            if (_confirmButton != null)
            {
                if (_cancelButton == null)
                {
                    _confirmButton.HorizontalAlignment = HorizontalAlignment.Center;
                    _confirmButton.Left = -(_confirmButton.Width / 2);
                }
                else
                {
                    _confirmButton.HorizontalAlignment = HorizontalAlignment.Right;
                    _confirmButton.Left = -(_confirmButton.Width + 4);
                }

                _confirmButton.VerticalAlignment = VerticalAlignment.Bottom;
                _confirmButton.Top = -(_confirmButton.Height + 4);
            }

            if (_cancelButton != null)
            {
                if (_confirmButton == null)
                {
                    _cancelButton.HorizontalAlignment = HorizontalAlignment.Center;
                    _cancelButton.Left = -(_cancelButton.Width / 2);
                }
                else
                {
                    _cancelButton.HorizontalAlignment = HorizontalAlignment.Left;
                    _cancelButton.Left = 4;
                }

                _cancelButton.VerticalAlignment = VerticalAlignment.Bottom;
                _cancelButton.Top = -(_cancelButton.Height + 4);
            }
        }

        public void Show()
        {
            _dialog.Show();
        }

        public void Hide()
        {
            _dialog.Hide();
        }

        private OverlayElementContainer CreateDialogElement()
        {
            var dialogElement = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElementFromTemplate("Overlays/Templates/MessageDialog", null, InstanceName);
            return dialogElement;
        }

        public void SetMessage(string message)
        {
            _message = message;
            var textField = (OverlayElement)OverlayManager.Instance.Elements.GetElement(InstanceName + "/MessageDialogContent/MessageDialogMessage");
            textField.Text = _message;
            ResizeElement();
        }

        public void ShowConfirmButton(string label, Action action)
        {
            _confirmButton = CreateConfirmButton(label);
            var dialogContent = (OverlayElementContainer)OverlayManager.Instance.Elements.GetElement(InstanceName + "/MessageDialogContent");
            dialogContent.AddChildElement(_confirmButton);
            _confirmButton.UserData = action;
            Globals.UI.AddButton(_confirmButton);
            ResizeElement();
        }

        private OverlayElementContainer CreateConfirmButton(string label)
        {
            var confirmButton = Globals.UI.CreateButton(InstanceName + "/ConfirmButton", "Green", label, (int)_dialogElement.Width / 2 - 10, 25, 18);
            return confirmButton;
        }

        private void DestroyConfirmButton()
        {
            if (_confirmButton != null)
            {
                _dialogElement.RemoveChild(_confirmButton.Name);
                if (_confirmButton.UserData != null)
                    Globals.UI.RemoveButton(_confirmButton);
                Globals.UI.DestroyButton(_confirmButton.Name);
            }
        }

        public void ShowCancelButton(string label, Action action)
        {
            _cancelButton = CreateCancelButton(label);
            var dialogContent = (OverlayElementContainer)OverlayManager.Instance.Elements.GetElement(InstanceName + "/MessageDialogContent");
            dialogContent.AddChildElement(_cancelButton);
            _cancelButton.UserData = action;
            Globals.UI.AddButton(_cancelButton);
            ResizeElement();
        }

        private OverlayElementContainer CreateCancelButton(string label)
        {
            var cancelButton = Globals.UI.CreateButton(InstanceName + "/CancelButton", "Blue", label, (int)_dialogElement.Width / 2 - 10, 25, 18);
            return cancelButton;
        }

        private void DestroyCancelButton()
        {
            if (_cancelButton != null)
            {
                _dialogElement.RemoveChild(_cancelButton.Name);
                if (_cancelButton.UserData != null)
                    Globals.UI.RemoveButton(_cancelButton);
                Globals.UI.DestroyButton(_cancelButton.Name);
            }
        }
    }
}
