using Core.Items;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class InventoryTest
    {
        private Inventory _inventory;

        [SetUp]
        public void Setup()
        {
            _inventory = new Inventory(Guid.NewGuid());
        }

        [Test]
        public void TestInitiallyEmpty()
        {
            Assert.IsEmpty(_inventory);
        }

        [Test]
        public void TestAdd()
        {
            var stack = new ItemStack(Guid.NewGuid(), ItemType.MiningDroid, 1);
            Assert.AreEqual(Guid.Empty, stack.ContainerID);
            _inventory.Add(stack);
            Assert.AreEqual(_inventory.ID, stack.ContainerID);
            CollectionAssert.AreEqual(
                new[] { Tuple.Create(ItemType.MiningDroid, 1) },
                _inventory.Select(x => Tuple.Create(x.Type, x.Count)));
        }

        [Test]
        public void TestRemove()
        {
            var id = Guid.NewGuid();
            var stack = new ItemStack(id, ItemType.MiningDroid, 1);
            _inventory.Add(stack);
            _inventory.Remove(id);
            Assert.AreEqual(Guid.Empty, stack.ContainerID);
            Assert.IsEmpty(_inventory);
        }

        [Test]
        public void TestRemove_NonexistentOk()
        {
            _inventory.Remove(Guid.NewGuid());
        }
    }
}
