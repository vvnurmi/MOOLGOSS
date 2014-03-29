using Axiom.Math;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Core
{
    public interface IPosed
    {
        Pose Pose { get; }
    }

    // Note: Pose implements ISerializable only to work around this serialization bug:
    // http://connect.microsoft.com/VisualStudio/feedback/details/312970/weird-argumentexception-when-deserializing-field-in-typedreferences-cannot-be-static-or-init-only
    [Serializable]
    public struct Pose : ISerializable
    {
        public readonly Vector3 Location;
        public readonly Vector3 Front;
        public readonly Vector3 Up;
        public Vector3 Right { get { return Front.Cross(Up).ToNormalized(); } }
        public Quaternion Orientation { get { return Quaternion.FromAxes(Right, Up, -Front); } }

        public Pose(Vector3 location, Vector3 front, Vector3 up)
        {
            Location = location;
            Front = front;
            Up = up;
        }

        private Pose(SerializationInfo info, StreamingContext context)
        {
            Location = (Vector3)info.GetValue("Location", typeof(Vector3));
            Front = (Vector3)info.GetValue("Front", typeof(Vector3));
            Up = (Vector3)info.GetValue("Up", typeof(Vector3));
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "(loc {0}, front {1}, up {2})", Location, Front, Up);
        }

        public Pose Move(Vector3 deltaLocation, float pitchDegrees, float yawDegrees, float rollDegrees)
        {
            var location = Location + deltaLocation;
            var pitchRotation = Quaternion.FromAngleAxis(Utility.DegreesToRadians(pitchDegrees), Right);
            var yawRotation = Quaternion.FromAngleAxis(Utility.DegreesToRadians(yawDegrees), Up);
            var rollRotation = Quaternion.FromAngleAxis(Utility.DegreesToRadians(rollDegrees), Front);
            var front = yawRotation * pitchRotation * Front;
            var up = rollRotation * pitchRotation * Up;
            return new Pose(location, front, up);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Location", Location);
            info.AddValue("Front", Front);
            info.AddValue("Up", Up);
        }
    }
}
