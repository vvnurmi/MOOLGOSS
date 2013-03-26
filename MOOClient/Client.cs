using MOO.Client.GUI;
using MOO.Client.MOOService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MOO.Client
{
    public class Client : Application
    {
        private MOOServiceClient _service;
        private Planet[] _planets;

        public Client()
        {
            _service = new MOOServiceClient("NetNamedPipeBinding_IMOOService1");
            _planets = new Planet[0];
        }

        //protected override void Update(GameTime gameTime)
        //{
        //    base.Update(gameTime);
        //    var keyboard = Keyboard.GetState();
        //    if (!_oldKeyboard.IsKeyDown(Keys.Space) && keyboard.IsKeyDown(Keys.Space))
        //        _planets = _service.GetPlanets();
        //    _oldKeyboard = keyboard;
        //}
    }

    public static class App
    {
        [STAThread]
        public static void Main(string[] args)
        {
            new Client().Run(new ClientWindow());
        }
    }
}
