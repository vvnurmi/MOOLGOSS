using Axiom.Math;
using Core.Items;
using System;

namespace Core
{
    public interface IService
    {
        void SendWorldPatch(Guid clientID, WorldDiff diff);
        WorldDiff ReceiveWorldPatch(Guid clientID);
        [Obsolete]
        Planet[] GetPlanets();
        [Obsolete]
        Station[] GetStations();
        [Obsolete]
        Ship[] GetShips();
        [Obsolete]
        Ship GetShip(Guid id);
        [Obsolete]
        void UpdateShip(Guid id, Vector3 pos, Vector3 front, Vector3 up);
        [Obsolete]
        Inventory GetInventory(Guid id);
        [Obsolete]
        void AddToInventory(Guid id, ItemStack stack);
    }
}
