using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using MOO.Client.MOOService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace MOO.Client
{
    public class Client : Game
    {
        private GraphicsDeviceManager _graphics;
        private ContentManager _content;
        private KeyboardState _oldKeyboard;
        private MOOServiceClient _service;
        private Planet[] _planets;

        public Client()
        {
            _graphics = new GraphicsDeviceManager(this);
            _content = new ContentManager(Services);
            _service = new MOOServiceClient("NetNamedPipeBinding_IMOOService1");
            _planets = new Planet[0];
        }
    
        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.Red);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var keyboard = Keyboard.GetState();
            if (!_oldKeyboard.IsKeyDown(Keys.Space) && keyboard.IsKeyDown(Keys.Space))
                _planets = _service.GetPlanets();
            _oldKeyboard = keyboard;
        }
    }

    public static class App
    {
        public static void Main(string[] args)
        {
            using (var game = new Client()) game.Run();
        }
    }
}
