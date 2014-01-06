using Axiom.Graphics;
using Axiom.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class Input : IDisposable
    {
        private InputReader _input;
        private bool[] _previousKeyStates;
        private bool[] _currentKeyStates;

        public bool MouseCapture { get; set; }
        public float RelativeMouseX { get { return _input.RelativeMouseX; } }
        public float RelativeMouseY { get { return _input.RelativeMouseY; } }
        /// <summary>
        /// Key events are obtained only for these keys. Hacky hack.
        /// </summary>
        public List<KeyCodes> KeyEventTargets { get; private set; }

        public Input()
        {
            _input = new Axiom.Platforms.Win32.Win32InputReader();
            var maxKeyCode = (int)Enum.GetValues(typeof(KeyCodes)).Cast<KeyCodes>().Max();
            _previousKeyStates = new bool[maxKeyCode];
            _currentKeyStates = new bool[maxKeyCode];
            MouseCapture = true;
            KeyEventTargets = new List<KeyCodes>();
        }

        public void Initialize(RenderWindow window)
        {
            _input.Initialize(window, true, true, false, MouseCapture);
        }

        public void Update()
        {
            Array.Copy(_currentKeyStates, _previousKeyStates, _currentKeyStates.Length);
            _input.Capture();
            Array.Clear(_currentKeyStates, 0, _currentKeyStates.Length);
            foreach (var key in KeyEventTargets) _currentKeyStates[(int)key] = _input.IsKeyPressed(key);
        }

        public bool IsKeyPressed(KeyCodes key)
        {
            return _input.IsKeyPressed(key);
        }

        public bool IsKeyDownEvent(KeyCodes key)
        {
            return !_previousKeyStates[(int)key] && _currentKeyStates[(int)key];
        }

        public void Dispose()
        {
            _input.Dispose();
        }
    }
}
