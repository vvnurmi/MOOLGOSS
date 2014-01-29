using Client;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class UserInterfaceTest
    {
        private UserInterface _ui;

        [SetUp]
        public void Setup()
        {
            _ui = new UserInterface();
        }

        [Test]
        public void UnknownModeThrows()
        {
            Assert.Throws<KeyNotFoundException>(() => _ui.SetMode("foobar"));
        }

        [Test]
        public void DoubleRegistrationThrows()
        {
            _ui.AddMode(new UIMode("foobar"));
            Assert.Throws<ArgumentException>(() => _ui.AddMode(new UIMode("foobar")));
        }

        [Test]
        public void RegisterAndSetMode()
        {
            _ui.AddMode(new UIMode("foobar"));
            _ui.SetMode("foobar");
            Assert.AreEqual("foobar", _ui.Mode);
        }

        [Test]
        public void EventsAreFired()
        {
            bool entered, updated, exited;
            entered = updated = exited = false;
            Action enter = () => entered = true;
            Action update = () => updated = true;
            Action exit = () => exited = true;
            Action<bool, bool, bool, Action> assertEvents = (expected_entered, expected_updated, expected_exited, op) =>
            {
                entered = updated = exited = false;
                op();
                Assert.AreEqual(
                    new[] { expected_entered, expected_updated, expected_exited },
                    new[] { entered, updated, exited });
            };
            assertEvents(false, false, false, () => _ui.AddMode(new UIMode("exit")));
            assertEvents(false, false, false, () => _ui.AddMode(new UIMode("foobar")
                { Enter = enter, Update = update, Exit = exit }));
            assertEvents(true, false, false, () => _ui.SetMode("foobar"));
            assertEvents(false, true, false, _ui.Update);
            assertEvents(false, false, true, () => _ui.SetMode("exit"));
        }
    }
}
