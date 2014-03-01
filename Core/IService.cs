using Axiom.Math;
using Core.Items;
using System;

namespace Core
{
    public interface IService
    {
        /// <summary>
        /// Returns true always.
        /// </summary>
        bool SendWorldPatch(Guid clientID, WorldDiff diff);
        WorldDiff ReceiveWorldPatch(Guid clientID);
    }
}
