using UnityEngine;
using System.Threading.Tasks;
using Game.ParticleModule.Config;
using Game.ParticleModule.Service;
using Cysharp.Threading.Tasks;

namespace Game.ParticleModule.Service
{
    public sealed class ParticleSpawner
    {
        private readonly ParticlePoolService poolService;
        private readonly ParticleConfig config;

        public ParticleSpawner(ParticlePoolService rPoolService, ParticleConfig rConfig)
        {
            poolService = rPoolService;
            config = rConfig;
        }

        public void SpawnBetween(Vector3 start, Vector3 end, ParticleType type)
        {
            Vector3 direction = end - start;
            Vector3 center = start + (direction * 0.5f);

            var ps = InternalSpawn(type, center);
            if (ps != null)
            {
                ps.transform.right = direction.normalized;
            }
        }

        public void SpawnDirectional(Vector3 position, Vector3 normal, ParticleType type)
        {
            var ps = InternalSpawn(type, position);

            if (ps != null)
            {
                ps.transform.up = normal;
            }
        }

        private ParticleSystem InternalSpawn(ParticleType type, Vector3 position)
        {
            ParticleData particleData = poolService.GetData(type);
            ParticleSystem particleSystem = poolService.Get(type);

            particleSystem.gameObject.SetActive(true);
            particleSystem.transform.position = position;
            particleSystem.Play(true);

            ReturnToPoolAfterDelay(type, particleSystem, particleData.defaultDuration).Forget();

            return particleSystem;
        }

        private async UniTaskVoid ReturnToPoolAfterDelay(ParticleType type, ParticleSystem ps, float delay)
        {
            await UniTask.Delay((int)(delay * 1000), delayTiming: PlayerLoopTiming.Update);

            if (ps != null && ps.gameObject != null)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                poolService.Return(type, ps);
            }
        }
    }
}