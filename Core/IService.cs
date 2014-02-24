using Axiom.Math;
using Core.Items;
using System;

namespace Core
{
    public interface IService
    {
        void SendWorldPatch(Guid clientID, WorldDiff diff);
        WorldDiff ReceiveWorldPatch(Guid clientID);
    }
}
