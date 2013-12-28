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
            CollectionAssert.AreEquivalent(new[] { "Earth" }, _service.GetPlanets().Select(x => x.Name));
        }
    }
}
