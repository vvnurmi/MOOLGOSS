using Axiom.Math;
using System;

namespace Core
{
    public interface IService
    {
        Planet[] GetPlanets();
        Ship[] GetShips();
        Ship GetShip(Guid id);
        void UpdateShip(Guid id, Vector3 pos, Vector3 front, Vector3 up);
    }
}
