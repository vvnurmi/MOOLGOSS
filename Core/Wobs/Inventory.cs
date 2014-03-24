using Core.Items;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.Wobs
{
    [Serializable]
    public sealed class Inventory : Wob, IEnumerable<ItemStack>, ISerializable
    {
        private readonly ImmutableDictionary<Guid, ItemStack> _stacks = ImmutableDictionary<Guid, ItemStack>.Empty;
        private readonly int _changeTimestamp;

        public int ChangeTimestamp { get { return _changeTimestamp; } }

        public Inventory(Guid id)
            : base(id)
        {
        }

        private Inventory(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _changeTimestamp = info.GetInt32("ChangeTimestamp");
            var stacks = (KeyValuePair<Guid, ItemStack>[])info.GetValue("Stacks", typeof(KeyValuePair<Guid, ItemStack>[]));
            _stacks = ImmutableDictionary<Guid, ItemStack>.Empty.AddRange(stacks);
        }

        private Inventory(Guid id, ImmutableDictionary<Guid, ItemStack> stacks, int changeTimestamp)
            : base(id)
        {
            _stacks = stacks;
            _changeTimestamp = changeTimestamp;
        }

        public override bool Equals(Wob other)
        {
            var inventory = other as Inventory;
            return inventory != null && ID == inventory.ID && _stacks.ValueEquals(inventory._stacks);
        }

        /// <summary>
        /// Throws <see cref="InvalidOperationException"/> if the stack is already contained in some inventory.
        /// </summary>
        public Inventory Add(ItemStack stack)
        {
            if (stack.ContainerID != Guid.Empty) throw new InvalidOperationException("The stack is already contained somewhere else");
            Debug.Assert(!_stacks.ContainsKey(stack.ID));
            stack.ContainerID = ID;
            return new Inventory(ID, _stacks.Add(stack.ID, stack), _changeTimestamp + 1);
        }

        public Inventory Remove(Guid id)
        {
            ItemStack stack;
            if (!_stacks.TryGetValue(id, out stack)) return this;
            stack.ContainerID = Guid.Empty;
            return new Inventory(ID, _stacks.Remove(id), _changeTimestamp + 1);
        }

        public IEnumerator<ItemStack> GetEnumerator()
        {
            return _stacks.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _stacks.Values.GetEnumerator();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ChangeTimestamp", ChangeTimestamp);
            info.AddValue("Stacks", _stacks.ToArray());
        }
    }
}
