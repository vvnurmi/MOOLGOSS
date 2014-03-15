using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    /// <summary>
    /// An object in a <see cref="World"/>.
    /// Subclasses must implement <see cref="IEquatable<T>"/>.
    /// </summary>
    [Serializable]
    public abstract class Wob : IEquatable<Wob>
    {
        private readonly Guid _id;

        public Guid ID { get { return _id; } }

        public Wob(Guid id)
        {
            _id = id;
        }

        protected Wob(SerializationInfo info, StreamingContext context)
        {
            _id = info.GetGuid("ID");
        }

        public abstract bool Equals(Wob other);

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddGuid("ID", ID);
        }

        public virtual Wob Update() { return this; }
    }
}
