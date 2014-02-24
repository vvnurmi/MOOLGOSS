using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Items
{
    [Serializable]
    public class ItemStack : IEquatable<ItemStack>
    {
        public Guid ID { get; private set; }
        /// <summary>
        /// ID of the container of the <see cref="ItemStack"/>
        /// or <see cref="Guid.Empty"/> if the stack is not contained in anything.
        /// </summary>
        public Guid ContainerID { get; set; }
        public ItemType Type { get; private set; }
        public int Count { get; private set; }

        public ItemStack(Guid id, ItemType type, int count)
        {
            Debug.Assert(count > 0);
            ID = id;
            Type = type;
            Count = count;
        }

        public bool Equals(ItemStack other)
        {
            return ID == other.ID && Type == other.Type && Count == other.Count;
        }

        public override string ToString()
        {
            return string.Format("ItemStack {0} with {1} times {2}", ID, Count, Type);
        }
    }
}
