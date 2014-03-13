using Axiom.Math;
using Core;
using Core.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Service : IService
    {
        private Atom<World> _world;
        private Dictionary<Guid, World> _worldShadows = new Dictionary<Guid, World>();

        public Service(Atom<World> world)
        {
            _world = world;
        }

        public bool SendWorldPatch(Guid clientID, WorldDiff diff)
        {
            ModifyShadow(clientID, w => w.Patch(diff));
            _world.Set(w => w.Patch(diff));
            return true;
        }

        public WorldDiff ReceiveWorldPatch(Guid clientID)
        {
            WorldDiff diff = null;
            ModifyShadow(clientID, shadow => shadow.Patch(diff = new WorldDiff(shadow, _world)));
            return diff;
        }

        private void ModifyShadow(Guid clientID, Func<World, World> f)
        {
            World shadow;
            _worldShadows[clientID] = f(_worldShadows.TryGetValue(clientID, out shadow) ? shadow : World.Empty);
        }
    }
}
