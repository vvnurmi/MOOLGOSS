using Core;
using System;
using System.Collections.Concurrent;

namespace Server
{
    internal class Service : IService
    {
        private Atom<World> _world;
        private ConcurrentDictionary<Guid, Atom<World>> _worldShadows = new ConcurrentDictionary<Guid, Atom<World>>();

        public Service(Atom<World> world)
        {
            _world = world;
        }

        public bool SendWorldPatch(Guid clientID, WorldDiff diff)
        {
            SetShadow(clientID, w => w.Patch(diff));
            _world.Set(w => w.Patch(diff));
            return true;
        }

        public WorldDiff ReceiveWorldPatch(Guid clientID)
        {
            WorldDiff diff = null;
            SetShadow(clientID, shadow => shadow.Patch(diff = new WorldDiff(shadow, _world)));
            return diff;
        }

        private void SetShadow(Guid clientID, Func<World, World> f)
        {
            _worldShadows.GetOrAdd(clientID, id => new Atom<World>(World.Empty)).Set(f);
        }
    }
}
