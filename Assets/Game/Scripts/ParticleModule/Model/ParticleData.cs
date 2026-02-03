using System.Collections.Generic;
using UnityEngine;

namespace Game.ParticleModule.Config
{
    [System.Serializable]
    public struct ParticleData
    {
        public ParticleType type;
        public ParticleSystem prefab;
        public int preWarmCount;
        public float defaultDuration;
    }
}