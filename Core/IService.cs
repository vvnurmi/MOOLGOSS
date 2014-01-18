using Axiom.Math;
using Core.Items;
using System;

namespace Core
{
    public interface IService
    {
        Planet[] GetPlanets();
        Station[] GetStations();
        Ship[] GetShips();
        Ship GetShip(Guid id);
        void UpdateShip(Guid id, Vector3 pos, Vector3 front, Vector3 up);
        Inventory GetInventory(Guid id);
        void AddToInventory(Guid id, ItemStack stack);
    }
}
