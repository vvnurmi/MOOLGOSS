﻿using Axiom.Core;
using Axiom.Framework.Configuration;
using Axiom.Graphics;
using Axiom.Input;
using Axiom.Math;
using Core;
using Core.Serial;
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
        private SpaceVisualization _visualization;
        private float _timeToUpdate;
        private Ship _ship;

        public void Start(bool userConfigure, string host)
        {
            Connect(host);
            var configuration = ConfigurationManagerFactory.CreateDefault();
            using (var root = new Root("MOOLGOSS.log"))
            using (Globals.Input = new Axiom.Platforms.Win32.Win32InputReader())
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
                Globals.Input.Initialize(window, true, true, false, true);
                TextureManager.Instance.DefaultMipmapCount = 5;
                ResourceGroupManager.Instance.AddResourceLocation("Media", "Folder", true);
                ResourceGroupManager.Instance.InitializeAllResourceGroups();
                Globals.Scene = root.CreateSceneManager(SceneType.Generic);
                CreateCamera(window);
                CreateSpace();
                _ship = new Ship(Guid.NewGuid(), Vector3.Zero, Vector3.UnitX, Vector3.UnitY);
                root.FrameStarted += FrameStartedHandler;
                root.StartRendering();
            }
        }

        private void FrameStartedHandler(object sender, FrameEventArgs args)
        {
            var input = Globals.Input;
            var camera = Globals.Camera;
            input.Capture();
            _ship.Yaw(-0.3f * input.RelativeMouseX);
            _ship.Pitch(-0.3f * input.RelativeMouseY);
            var move = Vector3.Zero;
            if (input.IsKeyPressed(KeyCodes.W)) move += _ship.Front;
            if (input.IsKeyPressed(KeyCodes.S)) move -= _ship.Front;
            if (input.IsKeyPressed(KeyCodes.A)) move -= _ship.Right;
            if (input.IsKeyPressed(KeyCodes.D)) move += _ship.Right;
            _ship.Move(move * 50 * args.TimeSinceLastFrame);
            if (input.IsKeyPressed(KeyCodes.Escape)) args.StopRendering = true;
            UpdateCamera();
            UpdateShips(args);
            _visualization.UpdateShip(_ship);
        }

        private void UpdateCamera()
        {
            float SMOOTHNESS = 0.94f; // To be slightly below one.
            var cameraTilt = Quaternion.FromAngleAxis(Utility.DegreesToRadians(-10), _ship.Right);
            var targetOrientation = cameraTilt * _ship.Orientation;
            Globals.Camera.Orientation = Quaternion.Nlerp(1 - SMOOTHNESS, Globals.Camera.Orientation, targetOrientation, true);
            var cameraRelativeGoal = -60 * _ship.Front + 20 * _ship.Up;
            var cameraRelative = Globals.Camera.Position - _ship.Pos;
            Globals.Camera.Position = _ship.Pos + SMOOTHNESS * cameraRelative + (1 - SMOOTHNESS) * cameraRelativeGoal;
        }

        private void UpdateShips(FrameEventArgs args)
        {
            _timeToUpdate -= args.TimeSinceLastFrame;
            if (_timeToUpdate > 0) return;
            _timeToUpdate = 1;
            _service.UpdateShip(_ship.ID, _ship.Pos, _ship.Front, _ship.Up);
            foreach (var ship in _service.GetShips()) _visualization.UpdateShip(ship);
        }

        private RenderWindow CreateRenderWindow()
        {
            var renderWindow = Root.Instance.Initialize(true, "MOOLGOSS");
            var dx9RenderWindow = renderWindow as Axiom.RenderSystems.DirectX9.D3DRenderWindow;
            if (dx9RenderWindow != null)
            {
                var handle = dx9RenderWindow.PresentationParameters.DeviceWindowHandle;
                var window = System.Windows.Forms.Control.FromHandle(handle).FindForm();
                window.WindowState = System.Windows.Forms.FormWindowState.Maximized;
                window.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
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

        private void CreateSpace()
        {
            _visualization = new SpaceVisualization();
            _visualization.Create(_service.GetPlanets());
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
                if (response.ContentLength <= 0) return null;
                var responseData = new byte[response.ContentLength];
                response.GetResponseStream().Read(responseData, 0, responseData.Length);
                return Serialization.Build<object>(responseData);
            }
        }
    }
}
