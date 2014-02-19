using Axiom.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    [Serializable]
    public class Ship : IEquatable<Ship>
    {
        public Guid ID { get; private set; }
        public Vector3 Pos { get; private set; }
        public Vector3 Front { get; private set; }
        public Vector3 Up { get; private set; }
        public Vector3 Right { get { return Front.Cross(Up).ToNormalized(); } }
        public Quaternion Orientation { get { return Quaternion.FromAxes(Right, Up, -Front); } }

        public Ship(Guid id, Vector3 pos, Vector3 front, Vector3 up)
        {
            ID = id;
            Set(pos, front, up);
        }

        public bool Equals(Ship other)
        {
            return ID == other.ID && Pos == other.Pos
                && Front == other.Front && Up == other.Up && Right == other.Right;
        }

        public void Set(Vector3 pos, Vector3 front, Vector3 up)
        {
            Pos = pos;
            Front = front;
            Up = up;
        }

        public void Move(Vector3 delta)
        {
            Pos += delta;
        }

        public void Pitch(float degrees)
        {
            var rotation = Quaternion.FromAngleAxis(Utility.DegreesToRadians(degrees), Right);
            Front = rotation * Front;
            Up = rotation * Up;
        }

        public void Yaw(float degrees)
        {
            var rotation = Quaternion.FromAngleAxis(Utility.DegreesToRadians(degrees), Up);
            Front = rotation * Front;
        }

        public void Roll(float degrees)
        {
            var rotation = Quaternion.FromAngleAxis(Utility.DegreesToRadians(degrees), Front);
            Up = rotation * Up;
        }
    }
}
