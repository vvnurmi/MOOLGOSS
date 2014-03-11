using System;
using System.Collections.Generic;
using System.Linq;
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

        public abstract bool Equals(Wob other);
    }
}
