using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Items
{
    [Serializable]
    public class Inventory : IEnumerable<ItemStack>
    {
        private Dictionary<Guid, ItemStack> _stacks = new Dictionary<Guid,ItemStack>();

        public Guid ID { get; private set; }
        public int ChangeTimestamp { get; private set; }

        public Inventory(Guid id)
        {
            ID = id;
        }

        /// <summary>
        /// Throws <see cref="InvalidOperationException"/> if the stack is already contained in some inventory.
        /// </summary>
        public void Add(ItemStack stack)
        {
            if (stack.ContainerID != Guid.Empty) throw new InvalidOperationException("The stack is already contained somewhere else");
            Debug.Assert(!_stacks.ContainsKey(stack.ID));
            _stacks.Add(stack.ID, stack);
            stack.ContainerID = ID;
            ChangeTimestamp++;
        }

        public void Remove(Guid id)
        {
            ItemStack stack;
            if (!_stacks.TryGetValue(id, out stack)) return;
            _stacks.Remove(id);
            stack.ContainerID = Guid.Empty;
            ChangeTimestamp++;
        }

        public IEnumerator<ItemStack> GetEnumerator()
        {
            return _stacks.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _stacks.Values.GetEnumerator();
        }
    }
}
