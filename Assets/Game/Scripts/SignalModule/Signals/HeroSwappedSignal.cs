
using UnityEngine;

namespace Game.Signal.Core
{
    public struct HeroSwappedSignal
    {
        public Vector3 PosA;
        public Vector3 PosB;
        public bool IsSameType;
    }
}