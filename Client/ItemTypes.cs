using Axiom.Math;
using Core;
using Core.Items;
using Core.Wobs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal enum ItemActivationResult
    {
        /// <summary>
        /// Nothing needs to be done.
        /// </summary>
        Nothing,

        /// <summary>
        /// The item was depleted and should be erased from its container.
        /// </summary>
        IsDepleted,
    }

    internal static class ItemTypes
    {
        public static string GetCategoryName(ItemType type)
        {
            switch (type)
            {
                case ItemType.MiningDroid: return "Construction";
                default: throw new NotImplementedException("GetCategoryName for " + type);
            }
        }

        public static string GetIconName(ItemType type)
        {
            switch (type)
            {
                case ItemType.MiningDroid: return "Default";
                default: throw new NotImplementedException("GetIconName for " + type);
            }
        }

        public static ItemActivationResult Activate(ItemType type, Atom<World> world)
        {
            var result = (ItemActivationResult?)null;
            switch (type)
            {
                case ItemType.MiningDroid:
                    world.Set(w =>
                    {
                        var playerShip = w.GetWob<Ship>(w.GetPlayerShipID(Globals.PlayerID));
                        var planet = w.Wobs.Values.OfType<Planet>()
                            .FirstOrDefault(p => IsCloseAhead(playerShip.Pose, p.Pos));
                        result = planet == null ? ItemActivationResult.Nothing : ItemActivationResult.IsDepleted;
                        if (planet == null) return w;
                        var droidInventoryID = Guid.NewGuid();
                        return w
                            .SetWob(new Inventory(droidInventoryID))
                            .SetWob(new Droid(Guid.NewGuid(), playerShip.Pose, droidInventoryID));
                    });
                    break;
                default: throw new NotImplementedException("Activate for " + type);
            }
            Debug.Assert(result.HasValue, "Bug: Forgot to set activation result for " + type);
            return result.Value;
        }

        private static bool IsCloseAhead(Pose pose, Vector3 target)
        {
            if (pose.Location.DistanceSquared(target) > 150 * 150) return false;
            return (target - pose.Location).ToNormalized().DifferenceLessThan(pose.Front, 0.7f);
        }
    }
}
