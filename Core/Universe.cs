using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Universe
    {
        private List<IEntity> _entities = new List<IEntity>();

        public void Add(IEntity entity)
        {
            _entities.Add(entity);
        }

        public IEnumerable<Planet> GetPlanets()
        {
            return _entities.OfType<Planet>();
        }
    }
}
