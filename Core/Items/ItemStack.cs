using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Items
{
    public class ItemStack
    {
        public Guid ID { get; private set; }
        public ItemType Type { get; private set; }
        public int Count { get; private set; }

        public ItemStack(Guid id, ItemType type, int count)
        {
            Debug.Assert(count > 0);
            ID = id;
            Type = type;
            Count = count;
        }

        public override string ToString()
        {
            return string.Format("ItemStack {0} with {1} stacks", ID, Count);
        }
    }
}
