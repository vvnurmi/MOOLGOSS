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
            _inventory.Add(new ItemStack(Guid.NewGuid(), ItemType.MiningDroid, 1));
            CollectionAssert.AreEqual(
                new[] { Tuple.Create(ItemType.MiningDroid, 1) },
                _inventory.Select(x => Tuple.Create(x.Type, x.Count)));
        }

        [Test]
        public void TestRemove()
        {
            var id = Guid.NewGuid();
            _inventory.Add(new ItemStack(id, ItemType.MiningDroid, 1));
            _inventory.Remove(id);
            Assert.IsEmpty(_inventory);
        }
    }
}
