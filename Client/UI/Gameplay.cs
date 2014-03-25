using Axiom.Input;
using Axiom.Math;
using Client.Views;
using Core;
using Core.Wobs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InventoryView = Client.Views.Inventory;
using InventoryModel = Core.Wobs.Inventory;
using MissionDialog = Client.Views.MessageDialog;

namespace Client.UI
{
    internal class Gameplay : UIMode
    {
        private const float ServerSyncInterval = 1;

        private Guid _clientID;
        private World _worldShadow;
        private IService _service;
        private SpaceVisualization _visualization;
        private Mission _mission;
        private InventoryModel _inventory;
        private InventoryView _inventoryView;
        private TopBar _topBarView;
        private IAsyncResult _shipUpdateHandle;
        private bool _exiting;
        private ConcurrentQueue<WorldDiff> _visualizationUpdates = new ConcurrentQueue<WorldDiff>();

        private Input Input { get { return Globals.Input; } }

        public Gameplay(IService service)
            : base("Gameplay")
        {
            _clientID = Guid.NewGuid();
            Globals.World = new Atom<World>(World.Empty);
            _worldShadow = World.Empty;
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
                _inventory = new InventoryModel(Guid.NewGuid())
                    .Add(new Core.Items.ItemStack(Guid.NewGuid(), Core.Items.ItemType.MiningDroid, 2)); // !!!
                Globals.World.Set(w => w.SetWob(_inventory));
                _inventoryView = new InventoryView("Player", 10, 10, 28, 5, _inventory);
            }

            if (_topBarView == null)
            {
                _topBarView = new TopBar("Space", "The Ancient Sector : First Space Station");
            }

            _topBarView.AddButton("dock", "DOCK (F1)", TryDocking);

            if (Globals.PlayerID == Guid.Empty)
            {
                Globals.PlayerID = Guid.NewGuid();
                var shipID = Guid.NewGuid();
                Globals.World.Set(w => w
                    .SetPlayerShipID(Globals.PlayerID, shipID)
                    .SetWob(new Ship(shipID, new Pose(Vector3.Zero, Vector3.UnitX, Vector3.UnitY))));
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
            var world = Globals.World.Value;
            var ship = world.GetWob<Ship>(world.GetPlayerShipID(Globals.PlayerID));
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
                var yawDegrees = -0.3f * Input.RelativeMouseX;
                var pitchDegrees = -0.3f * Input.RelativeMouseY;
                var roll = 0f;
                if (Input.IsKeyPressed(KeyCodes.Q)) roll--;
                if (Input.IsKeyPressed(KeyCodes.E)) roll++;
                var rollDegrees = roll * 45 * secondsPassed;
                var move = Vector3.Zero;
                if (Input.IsKeyPressed(KeyCodes.W)) move += ship.Pose.Front;
                if (Input.IsKeyPressed(KeyCodes.S)) move -= ship.Pose.Front;
                if (Input.IsKeyPressed(KeyCodes.A)) move -= ship.Pose.Right;
                if (Input.IsKeyPressed(KeyCodes.D)) move += ship.Pose.Right;
                var deltaPos = move * 25 * secondsPassed;
                Globals.World.Set(w => w.SetWob(ship.SetPose(ship.Pose.Move(deltaPos, pitchDegrees, yawDegrees, rollDegrees))));
            }
            UpdateCamera();
            UpdateMission();
            _inventoryView.SyncWithModel();
            _visualization.UpdateVessel(ship, 0);
            WorldDiff visualizationDiff;
            while (_visualizationUpdates.TryDequeue(out visualizationDiff))
                _visualization.Update(visualizationDiff, ServerSyncInterval);
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
            var world = Globals.World.Value;
            var ship = world.GetWob<Ship>(world.GetPlayerShipID(Globals.PlayerID));
            var cameraTilt = Quaternion.FromAngleAxis(Utility.DegreesToRadians(-10), ship.Pose.Right);
            var targetOrientation = cameraTilt * ship.Pose.Orientation;
            Globals.Camera.Orientation = Quaternion.Nlerp(1 - SMOOTHNESS, Globals.Camera.Orientation, targetOrientation, true);
            var cameraRelativeGoal = -6 * ship.Pose.Front + 1.7 * ship.Pose.Up;
            var cameraRelative = Globals.Camera.Position - ship.Pose.Location;
            Globals.Camera.Position = ship.Pose.Location + SMOOTHNESS * cameraRelative + (1 - SMOOTHNESS) * cameraRelativeGoal;
        }

        private void UpdateMission()
        {
            if (_mission == null) return;
            var world = Globals.World.Value;
            var playerShip = world.GetWob<Ship>(world.GetPlayerShipID(Globals.PlayerID));
            switch (_mission.State)
            {
                case MissionState.Open:
                    if (_mission.AssignVolume.Intersects(playerShip.Pose.Location))
                    {
                        _mission.Offer();
                        var missionDialog = new MissionDialog("MissionOfferDialog", 300);
                        missionDialog.SetMessage(_mission.AssignMessage);
                        missionDialog.ShowConfirmButton("Accept", () => { missionDialog.Hide(); missionDialog.Destroy(); missionDialog = null; _mission.Assign(); });
                        missionDialog.ShowCancelButton("Refuse", () => { missionDialog.Hide(); missionDialog.Destroy(); missionDialog = null; _mission.Suppress(); });
                        missionDialog.Show();
                    }
                    break;
                case MissionState.Offering: break;
                case MissionState.Suppressed: break;
                case MissionState.Assigned:
                    if (_mission.CompleteVolume.Intersects(playerShip.Pose.Location))
                    {
                        _mission.Complete();
                        var missionDialog = new MissionDialog("MissionCompleteDialog", 300);
                        missionDialog.SetMessage(_mission.CompleteMessage);
                        missionDialog.ShowConfirmButton("OK", () => { missionDialog.Hide(); missionDialog.Destroy(); missionDialog = null; });
                        missionDialog.Show();
                    }
                    break;
                case MissionState.Completed: break;
                default: throw new NotImplementedException();
            }
        }

        private void SyncWithServerLoop()
        {
            try
            {
                while (!_exiting) SyncWithServer();
            }
            catch (Exception e)
            {
                var errorDialog = new MessageDialog("Communication error", 600);
                errorDialog.SetMessage("An error occurred when communicating with the server.\n" + e.Message);
                errorDialog.ShowConfirmButton("OK, too bad!", errorDialog.Destroy);
                errorDialog.Show();
            }
        }

        private void SyncWithServer()
        {
            Thread.Sleep(TimeSpan.FromSeconds(ServerSyncInterval));
            var diffOut = new WorldDiff(_worldShadow, Globals.World);
            _worldShadow = _worldShadow.Patch(diffOut);
            if (!diffOut.IsEmpty) _visualizationUpdates.Enqueue(diffOut);
            _service.SendWorldPatch(_clientID, diffOut);
            var diffIn = _service.ReceiveWorldPatch(_clientID);
            if (!diffIn.IsEmpty) _visualizationUpdates.Enqueue(diffIn);
            _worldShadow = _worldShadow.Patch(diffIn);
            Globals.World.Set(w => w.Patch(diffIn));
            if (_mission == null)
                _mission = new Mission
                {
                    AssignMessage = "Go and find The Planet!\nThere'll be no reward.",
                    AssignVolume = new Sphere(Globals.World.Value.Wobs.Values.OfType<Station>().First().Pos, 50),
                    CompleteMessage = "You found the correct planet,\nnice!",
                    CompleteVolume = new Sphere(Globals.World.Value.Wobs.Values.OfType<Planet>().First().Pos, 80),
                };
        }

        private void TryDocking()
        {
            Globals.UI.SetMode("Docked");
        }
    }
}
