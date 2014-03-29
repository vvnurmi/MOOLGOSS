using Axiom.Math;
using System;
using System.Globalization;
using System.Linq;

namespace Core.Wobs
{
    [Serializable]
    public sealed class Droid : Wob, IPosed
    {
        private enum State { Idle, FindPlanet, GoToPlanet, OrbitPlanet };

        [Serializable]
        private struct Logic
        {
            public readonly State State;
            public readonly Guid PlanetID;
            public readonly float GatheringSeconds;

            public static Logic Idle() { return new Logic(State.Idle, Guid.Empty, 0); }
            public static Logic FindPlanet() { return new Logic(State.FindPlanet, Guid.Empty, 0); }
            public static Logic GoToPlanet(Guid planetID) { return new Logic(State.GoToPlanet, planetID, 0); }
            public static Logic OrbitPlanet(Guid planetID) { return new Logic(State.OrbitPlanet, planetID, 0); }

            private Logic(State state, Guid planetID, float gatheringSeconds)
            {
                State = state;
                PlanetID = planetID;
                GatheringSeconds = gatheringSeconds;
            }

            public override string ToString()
            {
                return string.Format(CultureInfo.InvariantCulture, "{0} (planet {1}, gathered {2} secs)",
                    State, PlanetID, GatheringSeconds);
            }

            public Logic SetGatheringSeconds(float gatheringSeconds)
            {
                return new Logic(State, PlanetID, gatheringSeconds);
            }
        }

        private readonly Pose _pose;
        private readonly Guid _inventoryID;
        private readonly Logic _logic;

        public Pose Pose { get { return _pose; } }
        public Guid InventoryID { get { return _inventoryID; } }

        public Droid(Guid id, Pose pose, Guid inventoryID)
            : this(id, pose, inventoryID, Logic.FindPlanet())
        { }

        private Droid(Guid id, Pose pose, Guid inventoryID, Logic logic)
            : base(id)
        {
            _pose = pose;
            _inventoryID = inventoryID;
            _logic = logic;
        }

        public override bool Equals(Wob other)
        {
            var d = other as Droid;
            return d != null && _pose.Equals(d._pose) && _inventoryID.Equals(d._inventoryID) && _logic.Equals(d._logic);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Droid {0} with inventory {1} doing {2} at {3}",
                ID, _inventoryID, _logic, _pose);
        }

        public Droid SetPose(Pose pose) { return new Droid(ID, pose, _inventoryID, _logic); }
        private Droid SetLogic(Logic logic) { return new Droid(ID, _pose, _inventoryID, logic); }

        public override Wob Update(float secondsPassed)
        {
            var speedStep = 5 * secondsPassed;
            switch (_logic.State)
            {
                case State.Idle: return this;
                case State.FindPlanet:
                    {
                        var planet = Globals.World.Value.Wobs.Values.OfType<Planet>()
                            .MinBy(p => (float)p.Pos.DistanceSquared(_pose.Location));
                        if (planet == null || planet.Pos.DistanceSquared(_pose.Location) > 300 * 300)
                            return SetLogic(Logic.Idle());
                        return SetLogic(Logic.GoToPlanet(planet.ID));
                    }
                case State.GoToPlanet:
                    {
                        var planet = Globals.World.Value.GetWob<Planet>(_logic.PlanetID);
                        if (planet == null || planet.Pos.DistanceSquared(_pose.Location) < 70 * 70)
                            return SetLogic(Logic.OrbitPlanet(_logic.PlanetID));
                        var toPlanet = (planet.Pos - _pose.Location).ToNormalized();
                        return SetPose(new Pose(_pose.Location + toPlanet * speedStep, toPlanet, _pose.Up));
                    }
                case State.OrbitPlanet:
                    {
                        var planet = Globals.World.Value.GetWob<Planet>(_logic.PlanetID);
                        if (planet == null) return SetLogic(Logic.Idle());
                        var orbitLocation = _pose.Location - planet.Pos;
                        var altitude = orbitLocation.Length;
                        var orbitAngle = speedStep / altitude;
                        var move = Quaternion.FromAngleAxis(orbitAngle, _pose.Up);
                        var location = planet.Pos + move * orbitLocation;
                        var gatheringSeconds = _logic.GatheringSeconds + secondsPassed;
                        var gatheringInterval = 5;
                        var gatheredAmount = (int)(gatheringSeconds / gatheringInterval);
                        if (gatheredAmount > 0)
                        {
                            var gatheredStack = new Items.ItemStack(Guid.NewGuid(), Items.ItemType.MiningDroid, gatheredAmount);
                            // FIXME !!! Inventory change should be a return value that is effected by the caller.
                            Globals.World.Set(w => w.SetWob(w.GetWob<Inventory>(_inventoryID).Add(gatheredStack)));
                        }
                        gatheringSeconds -= gatheredAmount * gatheringInterval;
                        return SetPose(new Pose(location, move * _pose.Front, move * _pose.Up))
                            .SetLogic(_logic.SetGatheringSeconds(gatheringSeconds));
                    }
                default: throw new NotImplementedException();
            }
        }
    }
}
