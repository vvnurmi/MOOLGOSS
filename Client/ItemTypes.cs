using Axiom.Math;
using Core;
using Core.Items;
using Core.Wobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal enum ItemActivationResult
    {
        /// <summary>
        /// The item was depleted and should be erased from its container.
        /// </summary>
        IsDepleted,
    }

    internal struct ItemActivation
    {
        public readonly ItemActivationResult Result;
        public readonly Func<World, World> Effect;

        public ItemActivation(ItemActivationResult result, Func<World, World> effect)
        {
            Result = result;
            Effect = effect;
        }
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

        public static ItemActivation Activate(ItemType type)
        {
            var result =
                type == ItemType.MiningDroid ? new ItemActivation(ItemActivationResult.IsDepleted, w =>
                {
                    var playerShip = w.GetWob<Ship>(w.GetPlayerShipID(Globals.PlayerID));
                    var pos = playerShip.Pos + playerShip.Front * 5;
                    return w.SetWob(new Droid(Guid.NewGuid(), pos, playerShip.Front, playerShip.Up));
                }) :
                (ItemActivation?)null;
            if (!result.HasValue) throw new NotImplementedException("Activate for " + type);
            return result.Value;
        }
    }
}
