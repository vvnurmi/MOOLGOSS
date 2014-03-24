using Axiom.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.Wobs
{
    [Serializable]
    public sealed class Ship : Wob, IPosed
    {
        private readonly Pose _pose;
        public Pose Pose { get { return _pose; } }

        public Ship(Guid id, Pose pose)
            : base(id)
        {
            _pose = pose;
        }

        public override bool Equals(Wob other)
        {
            var ship = other as Ship;
            return ship != null && ID == ship.ID && Pose.Equals(ship._pose);
        }

        public Ship SetPose(Pose pose) { return new Ship(ID, pose); }
    }
}
