using Axiom.Math;
using System;
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

            public static Logic Idle() { return new Logic(State.Idle, Guid.Empty); }
            public static Logic FindPlanet() { return new Logic(State.FindPlanet, Guid.Empty); }
            public static Logic GoToPlanet(Guid planetID) { return new Logic(State.GoToPlanet, planetID); }
            public static Logic OrbitPlanet(Guid planetID) { return new Logic(State.OrbitPlanet, planetID); }

            private Logic(State state, Guid planetID)
            {
                State = state;
                PlanetID = planetID;
            }
        }

        private readonly Pose _pose;
        private readonly Logic _logic;
        public Pose Pose { get { return _pose; } }

        public Droid(Guid id, Pose pose)
            : this(id, pose, Logic.FindPlanet())
        { }

        private Droid(Guid id, Pose pose, Logic logic)
            : base(id)
        {
            _pose = pose;
            _logic = logic;
        }

        public override bool Equals(Wob other)
        {
            var d = other as Droid;
            return d != null && _pose.Equals(d._pose) && _logic.Equals(d._logic);
        }

        public Droid SetPose(Pose pose) { return new Droid(ID, pose, _logic); }
        private Droid SetLogic(Logic logic) { return new Droid(ID, _pose, logic); }

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
                        return SetPose(new Pose(location, move * _pose.Front, move * _pose.Up));
                    }
                default: throw new NotImplementedException();
            }
        }
    }
}
