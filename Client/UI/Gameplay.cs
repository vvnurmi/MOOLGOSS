using Axiom.Input;
using Axiom.Math;
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InventoryView = Client.Views.Inventory;
using TopBarView = Client.Views.TopBar;
using InventoryModel = Core.Items.Inventory;

namespace Client.UI
{
    internal class Gameplay : UIMode
    {
        private Guid _clientID;
        private World _world;
        private World _worldShadow;
        private IService _service;
        private SpaceVisualization _visualization;
        private Mission _mission;
        private InventoryModel _inventory;
        private InventoryView _inventoryView;
        private TopBarView _topBarView;
        private IAsyncResult _shipUpdateHandle;
        private bool _exiting;

        private Input Input { get { return Globals.Input; } }

        public Gameplay(IService service)
            : base("Gameplay")
        {
            _clientID = Guid.NewGuid();
            _world = new World();
            _worldShadow = _world.Clone();
            _service = service;
            Enter = EnterHandler;
            Update = UpdateHandler;
            Exit = ExitHandler;
        }

        private void EnterHandler()
        {
            Globals.UI.HideMouse();

            if (_inventory == null)
            {
                _inventory = new InventoryModel(Guid.NewGuid());
                _world.AddInventory(_inventory);
                _inventory.Add(new Core.Items.ItemStack(Guid.NewGuid(), Core.Items.ItemType.MiningDroid, 2)); // !!!
                _inventoryView = new InventoryView("Player", 10, 10, 28, 5, _inventory);
            }

            if (_topBarView == null)
            {
                _topBarView = new TopBarView("Space", "The Ancient Sector : First Space Station");
            }

            _topBarView.AddButton("dock", "DOCK (F1)", TryDocking);

            if (Globals.PlayerShip == null)
            {
                Globals.PlayerShip = new Ship(Guid.NewGuid(), Vector3.Zero, Vector3.UnitX, Vector3.UnitY);
                _world.AddShip(Globals.PlayerShip);
            }

            if (_shipUpdateHandle == null)
            {
                _shipUpdateHandle = new Action(SyncWithServerLoop).BeginInvoke(null, null);
            }

            if (_visualization == null)
            {
                _visualization = new SpaceVisualization();
                _visualization.CreateStaticThings();
            }
        }

        private void UpdateHandler(float secondsPassed)
        {
            var ship = Globals.PlayerShip;
            if (!_topBarView.IsVisible)
                _topBarView.Show();
            if (Input.IsKeyDownEvent(KeyCodes.F1))
                TryDocking();
            if (Input.IsKeyDownEvent(KeyCodes.I))
                if (_inventoryView.IsVisible)
                    _inventoryView.Hide();
                else
                    _inventoryView.Show();
            if (Input.IsKeyDownEvent(KeyCodes.Space))
                if (Globals.UI.IsMouseVisible)
                    Globals.UI.HideMouse();
                else
                    Globals.UI.ShowMouse();
            if (!Globals.UI.IsMouseVisible)
            {
                ship.Yaw(-0.3f * Input.RelativeMouseX);
                ship.Pitch(-0.3f * Input.RelativeMouseY);
                var roll = 0f;
                if (Input.IsKeyPressed(KeyCodes.Q)) roll--;
                if (Input.IsKeyPressed(KeyCodes.E)) roll++;
                ship.Roll(roll * 45 * secondsPassed);
                var move = Vector3.Zero;
                if (Input.IsKeyPressed(KeyCodes.W)) move += ship.Front;
                if (Input.IsKeyPressed(KeyCodes.S)) move -= ship.Front;
                if (Input.IsKeyPressed(KeyCodes.A)) move -= ship.Right;
                if (Input.IsKeyPressed(KeyCodes.D)) move += ship.Right;
                ship.Move(move * 25 * secondsPassed);
            }
            UpdateCamera();
            UpdateMission();
            _inventoryView.SyncWithModel();
            _visualization.UpdateShip(ship, 0);
            _visualization.Update();
        }

        private void ExitHandler()
        {
            if (_topBarView != null && _topBarView.IsVisible)
            {
                _topBarView.RemoveButton("dock");
                _topBarView.Hide();
            }


            _exiting = true;
            _shipUpdateHandle.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2));
        }

        private void UpdateCamera()
        {
            float SMOOTHNESS = 0.90f; // To be slightly below one.
            var ship = Globals.PlayerShip;
            var cameraTilt = Quaternion.FromAngleAxis(Utility.DegreesToRadians(-10), ship.Right);
            var targetOrientation = cameraTilt * ship.Orientation;
            Globals.Camera.Orientation = Quaternion.Nlerp(1 - SMOOTHNESS, Globals.Camera.Orientation, targetOrientation, true);
            var cameraRelativeGoal = -6 * ship.Front + 1.7 * ship.Up;
            var cameraRelative = Globals.Camera.Position - ship.Pos;
            Globals.Camera.Position = ship.Pos + SMOOTHNESS * cameraRelative + (1 - SMOOTHNESS) * cameraRelativeGoal;
        }

        private void UpdateMission()
        {
            if (_mission == null) return;
            switch (_mission.State)
            {
                case MissionState.Open:
                    if (_mission.AssignVolume.Intersects(Globals.PlayerShip.Pos))
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
                    if (_mission.CompleteVolume.Intersects(Globals.PlayerShip.Pos))
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

        private void SyncWithServerLoop()
        {
            float updateInterval = 1;
            while (!_exiting)
            {
                // TODO: Thread safety. Two threads modify _world and Axiom's scene graph.
                Thread.Sleep(TimeSpan.FromSeconds(updateInterval));
                var diffOut = new WorldDiff(_worldShadow, _world);
                _worldShadow.Patch(diffOut);
                _visualization.Update(diffOut);
                _service.SendWorldPatch(_clientID, diffOut);
                var diffIn = _service.ReceiveWorldPatch(_clientID);
                _visualization.Update(diffIn);
                _worldShadow.Patch(diffIn);
                _world.Patch(diffIn);
                if (_mission == null)
                    _mission = new Mission
                    {
                        AssignMessage = "Go and find The Planet!\nThere'll be no reward.",
                        AssignVolume = new Sphere(_world.Stations.First().Value.Pos, 50),
                        CompleteMessage = "You found the correct planet,\nnice!",
                        CompleteVolume = new Sphere(_world.Planets.First().Value.Pos, 80),
                    };
            }
        }

        private void TryDocking()
        {
            Globals.UI.SetMode("Docked");
        }
    }
}
