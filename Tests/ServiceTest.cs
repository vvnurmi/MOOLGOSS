using Axiom.Math;
using Core;
using Core.Items;
using NUnit.Framework;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class ServiceTest
    {
        private Service _service = new Service();
        private IService PublicService { get { return _service; } }

        [Test]
        public void TestGetPlanets()
        {
            CollectionAssert.AreEqual(new[] { "Earth" }, PublicService.GetPlanets().Select(x => x.Name));
        }

        [Test]
        public void TestGetStations()
        {
            _service.AddStation(Guid.NewGuid(), new Vector3(100, 0, 200));
            _service.AddStation(Guid.NewGuid(), new Vector3(300, 0, 100));
            CollectionAssert.AreEqual(
                new[] { new Vector3(100, 0, 200), new Vector3(300, 0, 100) },
                PublicService.GetStations().Select(x => x.Pos));
        }

        [Test]
        public void TestShips()
        {
            CollectionAssert.IsEmpty(PublicService.GetShips());
            var id = Guid.NewGuid();
            Action<Vector3, Vector3, Vector3> setAndAssertShip = (pos, front, up) =>
            {
                PublicService.UpdateShip(id, pos, front, up);
                Ship ship = PublicService.GetShips().SingleOrDefault();
                Assert.IsNotNull(ship);
                var expected = Tuple.Create(id, pos, front, up);
                Assert.AreEqual(expected, Tuple.Create(ship.ID, ship.Pos, ship.Front, ship.Up));
            };
            setAndAssertShip(new Vector3(1, 2, 3), Vector3.UnitX, Vector3.UnitY);
            setAndAssertShip(new Vector3(2, 2, 3), Vector3.UnitY, Vector3.UnitZ);
        }

        [Test]
        public void TestInventory()
        {
            var id = Guid.NewGuid();
            Assert.IsEmpty(PublicService.GetInventory(id));
            PublicService.AddToInventory(id, new ItemStack(Guid.NewGuid(), ItemType.MiningDroid, 1));
        }
    }
}
