using Axiom.Math;
using Core;
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

        [Test]
        public void TestGetPlanets()
        {
            CollectionAssert.AreEqual(new[] { "Earth" }, _service.GetPlanets().Select(x => x.Name));
        }

        [Test]
        public void TestShips()
        {
            CollectionAssert.IsEmpty(_service.GetShips());
            var id = new Guid();
            Action<Vector3, Vector3, Vector3> setAndAssertShip = (pos, front, up) =>
            {
                _service.UpdateShip(id, pos, front, up);
                Ship ship = _service.GetShips().SingleOrDefault();
                Assert.IsNotNull(ship);
                var expected = Tuple.Create(id, pos, front, up);
                Assert.AreEqual(expected, Tuple.Create(ship.ID, ship.Pos, ship.Front, ship.Up));
            };
            setAndAssertShip(new Vector3(1, 2, 3), Vector3.UnitX, Vector3.UnitY);
            setAndAssertShip(new Vector3(2, 2, 3), Vector3.UnitY, Vector3.UnitZ);
        }
    }
}
