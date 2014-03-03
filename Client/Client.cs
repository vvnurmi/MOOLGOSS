using Axiom.Core;
using Axiom.Framework.Configuration;
using Axiom.Graphics;
using Axiom.Input;
using Axiom.Math;
using Client.UI;
using Core;
using Core.Items;
using Core.Serial;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public class Client
    {
        public void Start(bool userConfigure, string host)
        {
            // HACK: Use an English culture so that Axiom.Overlays.Elements.BorderPanel works.
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            // HACK: Get assembly Axiom.Platforms.Win32.dll loaded before any dynamically created assembly.
            // This is to avoid an exception getting thrown from the Root constructor.
            { var hack = typeof(Axiom.Platforms.Win32.Win32InputReader); }

            var service = Connect(host);
            var configuration = ConfigurationManagerFactory.CreateDefault();
            using (var root = new Root("MOOLGOSS.log"))
            using (Globals.Input = new Input())
            {
                root.RenderSystem = root.RenderSystems[0];
                root.RenderSystem.ConfigOptions["VSync"].Value = "Yes";
                var bestMode =
                    root.RenderSystem.ConfigOptions["Video Mode"].PossibleValues
                    .Where(x => x.Value.Contains("32-bit color"))
                    .LastOrDefault().Value;
                if (bestMode != null) root.RenderSystem.ConfigOptions["Video Mode"].Value = bestMode;
                if (userConfigure && !configuration.ShowConfigDialog(root)) return;
                var window = CreateRenderWindow();
                Globals.Input.Initialize(window);
                ResourceGroupManager.Instance.AddResourceLocation("Media", "Folder", true);
                ResourceGroupManager.Instance.InitializeAllResourceGroups();
                Globals.Scene = root.CreateSceneManager(SceneType.Generic);
                Globals.UI = new UserInterface();
                Globals.UI.AddMode(new TitleScreen());
                Globals.UI.AddMode(new Gameplay(service));
                Globals.UI.AddMode(new Docked());
                Globals.UI.SetMode("Title Screen");
                CreateCamera(window);
                root.FrameStarted += FrameStartedHandler;
                root.StartRendering();
            }
        }

        private void FrameStartedHandler(object sender, FrameEventArgs args)
        {
            // When the window loses focus, Axiom keeps itself busy firing zero-time frames. Sleep calms it down!
            if (args.TimeSinceLastFrame == 0) Thread.Sleep(TimeSpan.FromSeconds(0.1));

            Globals.TotalTime += args.TimeSinceLastFrame;
            var input = Globals.Input;
            input.Update();
            Globals.UI.Update(args.TimeSinceLastFrame);

            var dx9RenderWindow = Globals.Camera.Viewport.Target as Axiom.RenderSystems.DirectX9.D3DRenderWindow;
            if (dx9RenderWindow != null && dx9RenderWindow.IsClosed) args.StopRendering = true;
            if (input.IsKeyDownEvent(KeyCodes.Escape)) args.StopRendering = true;

            if (args.StopRendering) Globals.UI.SetMode(null);
        }

        private RenderWindow CreateRenderWindow()
        {
            var renderWindow = Root.Instance.Initialize(true, "MOOLGOSS");
            var dx9RenderWindow = renderWindow as Axiom.RenderSystems.DirectX9.D3DRenderWindow;
            if (dx9RenderWindow != null)
            {
                var handle = dx9RenderWindow.PresentationParameters.DeviceWindowHandle;
                var window = System.Windows.Forms.Control.FromHandle(handle).FindForm();
                window.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                window.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            }
            return renderWindow;
        }

        private void CreateCamera(RenderWindow window)
        {
            Globals.Camera = Globals.Scene.CreateCamera("camera");
            var viewport = window.AddViewport(Globals.Camera);
            viewport.BackgroundColor = ColorEx.Black;
            Globals.Camera.Near = 1;
            Globals.Camera.Far = 2000;
            Globals.Camera.FieldOfView = Utility.DegreesToRadians(55);
            Globals.Camera.AspectRatio = viewport.ActualWidth / (float)viewport.ActualHeight;
            Globals.Camera.Position = new Vector3(-100, 100, 0);
            Globals.Camera.LookAt(Vector3.Zero);
        }

        private IService Connect(string host)
        {
            return Marshal.Get<IService>((method, args) => Invoke(host, method, args));
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
                if (response.ContentLength <= 0) return null;
                var responseData = new byte[response.ContentLength];
                response.GetResponseStream().ReadTo(responseData);
                return Serialization.Build<object>(responseData);
            }
        }
    }
}
