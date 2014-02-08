using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Items
{
    public enum ItemType
    {
        MiningDroid,
    }

    public static class ItemTypes
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
    }
}
