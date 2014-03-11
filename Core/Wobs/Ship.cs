using Axiom.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Wobs
{
    [Serializable]
    public class Ship : Wob
    {
        private readonly Vector3 _pos, _front, _up;

        public Vector3 Pos { get { return _pos; } }
        public Vector3 Front { get { return _front; } }
        public Vector3 Up { get { return _up; } }
        public Vector3 Right { get { return Front.Cross(Up).ToNormalized(); } }
        public Quaternion Orientation { get { return Quaternion.FromAxes(Right, Up, -Front); } }

        public Ship(Guid id, Vector3 pos, Vector3 front, Vector3 up)
            : base(id)
        {
            _pos = pos;
            _front = front;
            _up = up;
        }

        public override bool Equals(Wob other)
        {
            var ship = other as Ship;
            return ship != null && ID == ship.ID && Pos == ship.Pos
                && Front == ship.Front && Up == ship.Up && Right == ship.Right;
        }

        public Ship Set(Vector3 pos, Vector3 front, Vector3 up)
        {
            return new Ship(ID, pos, front, up);
        }

        public Ship Move(Vector3 deltaPos, float pitchDegrees, float yawDegrees, float rollDegrees)
        {
            var pos = Pos + deltaPos;
            var pitchRotation = Quaternion.FromAngleAxis(Utility.DegreesToRadians(pitchDegrees), Right);
            var yawRotation = Quaternion.FromAngleAxis(Utility.DegreesToRadians(yawDegrees), Up);
            var rollRotation = Quaternion.FromAngleAxis(Utility.DegreesToRadians(rollDegrees), Front);
            var front = yawRotation * pitchRotation * Front;
            var up = rollRotation * pitchRotation * Up;
            return Set(pos, front, up);
        }
    }
}
