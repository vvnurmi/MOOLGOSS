﻿using Axiom.Input;
using Axiom.Math;
using Core;
using Core.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client.UI
{
    internal class Gameplay : UIMode
    {
        private IService _service;
        private SpaceVisualization _visualization;
        private Ship _ship;
        private Mission _mission;
        private Inventory _inventory;
        private IAsyncResult _shipUpdateHandle;
        private bool _exiting;

        private Input Input { get { return Globals.Input; } }

        public Gameplay(IService service)
            : base("Gameplay")
        {
            _service = service;
            Enter = EnterHandler;
            Update = UpdateHandler;
            Exit = ExitHandler;
        }

        private void EnterHandler()
        {
            _inventory = _service.GetInventory(Guid.NewGuid());
            _ship = new Ship(Guid.NewGuid(), Vector3.Zero, Vector3.UnitX, Vector3.UnitY);
            _shipUpdateHandle = new Action(UpdateShipsLoop).BeginInvoke(null, null);
            CreateSpace();
        }

        private void UpdateHandler(float secondsPassed)
        {
            if (Input.IsKeyDownEvent(KeyCodes.I))
                if (Globals.UI.IsPlayerInventoryVisible)
                    Globals.UI.HidePlayerInventory();
                else
                    Globals.UI.ShowPlayerInventory();
            if (Input.IsKeyDownEvent(KeyCodes.Space))
                if (Globals.UI.IsMouseVisible)
                    Globals.UI.HideMouse();
                else
                    Globals.UI.ShowMouse();
            if (!Globals.UI.IsMouseVisible)
            {
                _ship.Yaw(-0.3f * Input.RelativeMouseX);
                _ship.Pitch(-0.3f * Input.RelativeMouseY);
                var roll = 0f;
                if (Input.IsKeyPressed(KeyCodes.Q)) roll--;
                if (Input.IsKeyPressed(KeyCodes.E)) roll++;
                _ship.Roll(roll * 45 * secondsPassed);
                var move = Vector3.Zero;
                if (Input.IsKeyPressed(KeyCodes.W)) move += _ship.Front;
                if (Input.IsKeyPressed(KeyCodes.S)) move -= _ship.Front;
                if (Input.IsKeyPressed(KeyCodes.A)) move -= _ship.Right;
                if (Input.IsKeyPressed(KeyCodes.D)) move += _ship.Right;
                _ship.Move(move * 25 * secondsPassed);
            }
            UpdateCamera();
            UpdateMission();
            _visualization.UpdateShip(_ship, 0);
            _visualization.Update();
        }

        private void ExitHandler()
        {
            _exiting = true;
            _shipUpdateHandle.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2));
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
                            new ButtonDef { Name = "Refuse", Pressed = () => { Globals.UI.HideDialog(); _mission.Suppress(); } },
                            new ButtonDef { Name = "Accept", Pressed = () => { Globals.UI.HideDialog(); _mission.Assign(); } });
                    }
                    break;
                case MissionState.Offering: break;
                case MissionState.Suppressed: break;
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
            while (!_exiting)
            {
                Thread.Sleep(TimeSpan.FromSeconds(updateInterval));
                _service.UpdateShip(_ship.ID, _ship.Pos, _ship.Front, _ship.Up);
                foreach (var ship in _service.GetShips())
                    if (ship.ID != _ship.ID) _visualization.UpdateShip(ship, updateInterval);
            }
        }
    }
}