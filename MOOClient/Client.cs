using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.Client
{
    public class Client : Game
    {
        private GraphicsDeviceManager _graphics;
        private ContentManager _content;

        public Client()
        {
            _graphics = new GraphicsDeviceManager(this);
            _content = new ContentManager(Services);
        }
    
        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.Red);
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
