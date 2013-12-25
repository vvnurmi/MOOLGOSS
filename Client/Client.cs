using Axiom.Core;
using Axiom.Framework.Configuration;
using Axiom.Graphics;
using Axiom.Input;
using Axiom.Math;
using Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Client
    {
        private IService _service;

        public void Start(bool userConfigure, string host)
        {
            Connect(host);
            var configuration = ConfigurationManagerFactory.CreateDefault();
            using (var root = new Root("MOOLGOSS.log"))
            using (Globals.Input = new Axiom.Platforms.Win32.Win32InputReader())
            {
                root.RenderSystem = root.RenderSystems[0];
                root.RenderSystem.IsVSync = true;
                if (userConfigure && !configuration.ShowConfigDialog(root)) return;
                var window = root.Initialize(true, "MOOLGOSS");
                Globals.Input.Initialize(window, true, true, false, false);
                TextureManager.Instance.DefaultMipmapCount = 5;
                ResourceGroupManager.Instance.AddResourceLocation("Media", "Folder", true);
                ResourceGroupManager.Instance.InitializeAllResourceGroups();
                Globals.Scene = root.CreateSceneManager(SceneType.Generic);
                CreateCamera(window);
                CreateSpace();
                root.FrameStarted += FrameStartedHandler;
                root.StartRendering();
            }
        }

        private void CreateSpace()
        {
            var space = new SpaceVisualization();
            space.Create(_service.GetPlanets());
        }

        private void FrameStartedHandler(object sender, FrameEventArgs args)
        {
            var input = Globals.Input;
            var camera = Globals.Camera;
            input.Capture();
            var angle = -0.3f * input.RelativeMouseX;
            camera.Yaw(angle);
            camera.Pitch(-0.3f * input.RelativeMouseY);
            var move = Vector3.Zero;
            if (input.IsKeyPressed(KeyCodes.W)) move -= Vector3.UnitZ;
            if (input.IsKeyPressed(KeyCodes.S)) move += Vector3.UnitZ;
            if (input.IsKeyPressed(KeyCodes.A)) move -= Vector3.UnitX;
            if (input.IsKeyPressed(KeyCodes.D)) move += Vector3.UnitX;
            camera.MoveRelative(move * 50 * args.TimeSinceLastFrame);
            if (input.IsKeyPressed(KeyCodes.Escape)) args.StopRendering = true;
        }

        private void CreateCamera(RenderWindow window)
        {
            Globals.Camera = Globals.Scene.CreateCamera("camera");
            var viewport = window.AddViewport(Globals.Camera, 0, 0, 1, 1, 10);
            viewport.BackgroundColor = ColorEx.Black;
            Globals.Camera.Near = 1;
            Globals.Camera.Far = 2000;
            Globals.Camera.Position = new Vector3(50, 250, 0);
            Globals.Camera.LookAt(new Vector3(450, 0, 100));
        }

        private void Connect(string host)
        {
            Debug.Assert(_service == null);
            _service = Marshal.Get<IService>((method, args) => Invoke(host, method, args));
        }

        private static object Invoke(string host, string method, object[] args)
        {
            var data = Serialization.Break(new MarshalledCall(method, args));
            var request = WebRequest.Create(string.Format("http://{0}:8080/moolgoss/", host));
            request.Method = "POST";
            request.ContentLength = data.Length;
            using (var requestStream = request.GetRequestStream())
                requestStream.Write(data, 0, data.Length);
            using (var response = request.GetResponse())
            {
                var responseData = new byte[response.ContentLength];
                response.GetResponseStream().Read(responseData, 0, responseData.Length);
                return Serialization.Build<object>(responseData);
            }
        }
    }
}
