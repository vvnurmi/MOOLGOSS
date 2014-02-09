using Axiom.Math;
using Core.Items;
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
                    //!!! Globals.World.Droids.Add(pos);
                    return ItemActivationResult.IsDepleted;
                default: throw new NotImplementedException("Activate for " + type);
            }
        }
    }
}
