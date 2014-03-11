using Axiom.Math;
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

        public static ItemActivationResult Activate(ItemType type, Vector3 pos)
        {
            switch (type)
            {
                case ItemType.MiningDroid:
                    Globals.World.Set(w => w.SetWob(new Droid(Guid.NewGuid(), pos, Vector3.UnitX, Vector3.UnitY)));
                    return ItemActivationResult.IsDepleted;
                default: throw new NotImplementedException("Activate for " + type);
            }
        }
    }
}
