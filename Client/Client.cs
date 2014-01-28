﻿using Axiom.Core;
using Axiom.Framework.Configuration;
using Axiom.Graphics;
using Axiom.Input;
using Axiom.Math;
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
        private IService _service;
        private SpaceVisualization _visualization;
        private Ship _ship;
        private Mission _mission;
        private Inventory _inventory;
        private bool _shuttingDown;
        private IAsyncResult _shipUpdateHandle;

        public void Start(bool userConfigure, string host)
        {
            // HACK: Use an English culture so that Axiom.Overlays.Elements.BorderPanel works.
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            // HACK: Get assembly Axiom.Platforms.Win32.dll loaded before any dynamically created assembly.
            // This is to avoid an exception getting thrown from the Root constructor.
            { var hack = typeof(Axiom.Platforms.Win32.Win32InputReader); }

            Connect(host);
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
                Globals.Input.KeyEventTargets.Add(KeyCodes.Space);
                ResourceGroupManager.Instance.AddResourceLocation("Media", "Folder", true);
                ResourceGroupManager.Instance.InitializeAllResourceGroups();
                Globals.Scene = root.CreateSceneManager(SceneType.Generic);
                Globals.UI = new UserInterface();
                CreateCamera(window);
                CreateSpace();
                _inventory = _service.GetInventory(Guid.NewGuid());
                _ship = new Ship(Guid.NewGuid(), Vector3.Zero, Vector3.UnitX, Vector3.UnitY);
                _shipUpdateHandle = new Action(UpdateShipsLoop).BeginInvoke(null, null);
                root.FrameStarted += FrameStartedHandler;
                root.FrameStarted += _visualization.FrameStartHandler;
                root.StartRendering();
                _shipUpdateHandle.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2));
            }
        }

        private void FrameStartedHandler(object sender, FrameEventArgs args)
        {
            // When the window loses focus, Axiom keeps itself busy firing zero-time frames. Sleep calms it down!
            if (args.TimeSinceLastFrame == 0) Thread.Sleep(TimeSpan.FromSeconds(0.1));

            Globals.TotalTime += args.TimeSinceLastFrame;
            var input = Globals.Input;
            input.Update();
            Globals.UI.Update();
            if (input.IsKeyDownEvent(KeyCodes.Space))
                if (Globals.UI.IsMouseVisible)
                    Globals.UI.HideMouse();
                else
                    Globals.UI.ShowMouse();
            if (!Globals.UI.IsMouseVisible)
            {
                _ship.Yaw(-0.3f * input.RelativeMouseX);
                _ship.Pitch(-0.3f * input.RelativeMouseY);
                var roll = 0f;
                if (input.IsKeyPressed(KeyCodes.Q)) roll--;
                if (input.IsKeyPressed(KeyCodes.E)) roll++;
                _ship.Roll(roll * 45 * args.TimeSinceLastFrame);
                var move = Vector3.Zero;
                if (input.IsKeyPressed(KeyCodes.W)) move += _ship.Front;
                if (input.IsKeyPressed(KeyCodes.S)) move -= _ship.Front;
                if (input.IsKeyPressed(KeyCodes.A)) move -= _ship.Right;
                if (input.IsKeyPressed(KeyCodes.D)) move += _ship.Right;
                _ship.Move(move * 25 * args.TimeSinceLastFrame);
            }
            UpdateCamera();
            UpdateMission();
            _visualization.UpdateShip(_ship, 0);

            var dx9RenderWindow = Globals.Camera.Viewport.Target as Axiom.RenderSystems.DirectX9.D3DRenderWindow;
            if (dx9RenderWindow != null && dx9RenderWindow.IsClosed) args.StopRendering = true;
            if (input.IsKeyPressed(KeyCodes.Escape)) args.StopRendering = true;

            if (!Globals.UI.IsTitleScreenConfirmed())
            {
                if (input.IsKeyPressed(KeyCodes.Enter) || input.IsKeyPressed(KeyCodes.Space))
                {
                    Globals.UI.ConfirmTitleScreen();
                }

                Globals.UI.TryShowTitleScreen();
            }
            else
            {
                if (input.IsKeyPressed(KeyCodes.I))
                {
                    if (Globals.UI.IsInventoryVisible())
                    {
                        Globals.UI.HideInventory();
                    }
                    else
                    {
                        Globals.UI.TryShowInventory();
                    }
                }
            }

            _shuttingDown = args.StopRendering;
        }

        private void UpdateCamera()
        {
            float SMOOTHNESS = 0.90f; // To be slightly below one.
            var cameraTilt = Quaternion.FromAngleAxis(Utility.DegreesToRadians(-10), _ship.Right);
            var targetOrientation = cameraTilt * _ship.Orientation;
            Globals.Camera.Orientation = Quaternion.Nlerp(1 - SMOOTHNESS, Globals.Camera.Orientation, targetOrientation, true);
            var cameraRelativeGoal = -6 * _ship.Front + 1.7 * _ship.Up;
            var cameraRelative = Globals.Camera.Position - _ship.Pos;
            Globals.Camera.Position = _ship.Pos + SMOOTHNESS * cameraRelative + (1 - SMOOTHNESS) * cameraRelativeGoal;
        }

        private void UpdateMission()
        {
            switch (_mission.State)
            {
                case MissionState.Open:
                    if (_mission.AssignVolume.Intersects(_ship.Pos))
                    {
                        _mission.Offer();
                        Globals.UI.TryShowDialog(_mission.AssignMessage,
                            new ButtonDef { Name = "Refuse", Pressed = () => { Globals.UI.HideDialog(); _mission.Unoffer(); } },
                            new ButtonDef { Name = "Accept", Pressed = () => { Globals.UI.HideDialog(); _mission.Assign(); } });
                    }
                    break;
                case MissionState.Offering: break;
                case MissionState.Assigned:
                    if (_mission.CompleteVolume.Intersects(_ship.Pos))
                    {
                        _mission.Complete();
                        Globals.UI.TryShowDialog(_mission.CompleteMessage,
                            new ButtonDef { Name = "OK", Pressed = Globals.UI.HideDialog });
                    }
                    break;
                case MissionState.Completed: break;
                default: throw new NotImplementedException();
            }
        }

        private void UpdateShipsLoop()
        {
            float updateInterval = 1;
            while (!_shuttingDown)
            {
                Thread.Sleep(TimeSpan.FromSeconds(updateInterval));
                _service.UpdateShip(_ship.ID, _ship.Pos, _ship.Front, _ship.Up);
                foreach (var ship in _service.GetShips())
                    if (ship.ID != _ship.ID) _visualization.UpdateShip(ship, updateInterval);
            }
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

        private void CreateSpace()
        {
            _visualization = new SpaceVisualization();
            var planets = _service.GetPlanets();
            var stations = _service.GetStations();
            _visualization.Create(planets, stations);
            _mission = new Mission
            {
                AssignMessage = "Go and find a planet!\nThere'll be no reward.",
                AssignVolume = new Sphere(stations[0].Pos, 50),
                CompleteMessage = "You found the planet, nice!",
                CompleteVolume = new Sphere(planets[0].Pos, 80),
            };
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
