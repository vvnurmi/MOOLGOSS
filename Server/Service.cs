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
        private World _world;
        private Dictionary<Guid, World> _worldShadows = new Dictionary<Guid, World>();

        public Service(World world)
        {
            _world = world;
        }

        public void SendWorldPatch(Guid clientID, WorldDiff diff)
        {
            GetShadow(clientID).Patch(diff);
            _world.Patch(diff);
        }

        public WorldDiff ReceiveWorldPatch(Guid clientID)
        {
            var shadow = GetShadow(clientID);
            var diff = new WorldDiff(shadow, _world);
            shadow.Patch(diff);
            return diff;
        }

        private World GetShadow(Guid clientID)
        {
            World shadow;
            if (!_worldShadows.TryGetValue(clientID, out shadow))
                _worldShadows.Add(clientID, shadow = new World());
            return shadow;
        }
    }
}
