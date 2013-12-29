using Axiom.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.Serial
{
    public class Vector3Surrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var v = (Vector3)obj;
            info.AddValue("x", v.x);
            info.AddValue("y", v.y);
            info.AddValue("z", v.z);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            return new Vector3(info.GetSingle("x"), info.GetSingle("y"), info.GetSingle("z"));
        }
    }
}
