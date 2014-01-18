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

        public Inventory(Guid id)
        {
            ID = id;
        }

        public void Add(ItemStack stack)
        {
            Debug.Assert(!_stacks.ContainsKey(stack.ID));
            _stacks.Add(stack.ID, stack);
        }

        public void Remove(Guid id)
        {
            _stacks.Remove(id);
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
