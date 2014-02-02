using Axiom.Graphics;
using Axiom.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class Input : IDisposable
    {
        private InputReader _input;
        private bool[] _previousMouseStates;
        private bool[] _currentMouseStates;
        private bool[] _previousKeyStates;
        private bool[] _currentKeyStates;
        /// <summary>
        /// Key events are obtained only for these keys. Hacky hack.
        /// </summary>
        private List<KeyCodes> _keyEventTargets = new List<KeyCodes>();

        public float RelativeMouseX { get { return _input.RelativeMouseX; } }
        public float RelativeMouseY { get { return _input.RelativeMouseY; } }
        public float AbsoluteMouseX { get { return _input.AbsoluteMouseX; } }
        public float AbsoluteMouseY { get { return _input.AbsoluteMouseY; } }

        public Input()
        {
            _input = new Axiom.Platforms.Win32.Win32InputReader();
            var maxKeyCode = (int)Enum.GetValues(typeof(KeyCodes)).Cast<KeyCodes>().Max();
            _previousKeyStates = new bool[maxKeyCode];
            _currentKeyStates = new bool[maxKeyCode];
            var maxMouseCode = (int)Enum.GetValues(typeof(MouseButtons)).Cast<MouseButtons>().Max();
            _previousMouseStates = new bool[maxMouseCode + 1];
            _currentMouseStates = new bool[maxMouseCode + 1];
        }

        public void Initialize(RenderWindow window)
        {
            _input.Initialize(window, true, true, false, true);
        }

        public void Update()
        {
            Array.Copy(_currentMouseStates, _previousMouseStates, _currentMouseStates.Length);
            Array.Copy(_currentKeyStates, _previousKeyStates, _currentKeyStates.Length);
            _input.Capture();
            Array.Clear(_currentMouseStates, 0, _currentMouseStates.Length);
            Array.Clear(_currentKeyStates, 0, _currentKeyStates.Length);
            foreach (MouseButtons button in Enum.GetValues(typeof(MouseButtons)))
                _currentMouseStates[(int)button] = _input.IsMousePressed(button);
            foreach (var key in _keyEventTargets)
                _currentKeyStates[(int)key] = _input.IsKeyPressed(key);
        }

        public bool IsMouseDownEvent(MouseButtons button)
        {
            Debug.Assert(button == MouseButtons.Left || button == MouseButtons.Middle || button == MouseButtons.Right);
            return !_previousMouseStates[(int)button] && _currentMouseStates[(int)button];
        }

        public bool IsKeyPressed(KeyCodes key)
        {
            return _input.IsKeyPressed(key);
        }

        public bool IsKeyDownEvent(KeyCodes key)
        {
            if (!_keyEventTargets.Contains(key)) _keyEventTargets.Add(key);
            return !_previousKeyStates[(int)key] && _currentKeyStates[(int)key];
        }

        public void SetMousePosition(int x, int y)
        {
#if !WINDOWS
#error SetMousePosition implemented only on Windows
#endif
            SetCursorPos(x, y);
        }

        public void Dispose()
        {
            _input.Dispose();
        }

#if WINDOWS
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);
#endif
    }
}
