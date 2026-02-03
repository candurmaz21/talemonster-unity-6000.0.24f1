using System.Collections.Generic;
using UnityEngine;
using Game.PoolSystem;
using Game.ParticleModule.Config;

namespace Game.ParticleModule.Service
{
    public sealed class ParticlePoolService
    {
        private readonly Dictionary<ParticleType, GameObjectPool<ParticleSystem>> pools = new();
        private readonly Dictionary<ParticleType, ParticleData> dataCache = new();
        private readonly ParticleConfig config;

        public ParticlePoolService(ParticleConfig rConfig, Transform rParticlePool)
        {
            foreach (var data in rConfig.particles)
            {
                var pool = new GameObjectPool<ParticleSystem>(data.prefab, rParticlePool, data.preWarmCount);
                pools.Add(data.type, pool);

                dataCache.Add(data.type, data);
            }
        }

        public ParticleSystem Get(ParticleType type) => pools[type].Get();
        public ParticleData GetData(ParticleType type) => dataCache[type];

        public void Return(ParticleType type, ParticleSystem ps)
        {
            if (ps == null)
            {
                return;
            }

            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            pools[type].Return(ps);
        }
    }
}