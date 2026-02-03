using Game.Signal.Core;
using Game.ParticleModule.Service;
using Game.ParticleModule.Config;
using VContainer.Unity;
using UnityEngine;
using System;

namespace Game.ParticleModule.Controller
{
    public sealed class ParticleController : IInitializable, IDisposable
    {
        private readonly ParticleSpawner spawner;
        private readonly SignalCenter signalCenter;

        public ParticleController(ParticleSpawner rSpawner, SignalCenter rSignalCenter)
        {
            spawner = rSpawner;
            signalCenter = rSignalCenter;
        }

        void IInitializable.Initialize()
        {
            signalCenter.Subscribe<HeroSwappedSignal>(OnHeroSwapped);
            signalCenter.Subscribe<HeroMergedSignal>(OnMatchResolved);
        }
        private void OnHeroSwapped(HeroSwappedSignal signal)
        {
            if (signal.IsSameType)
            {
                spawner.SpawnBetween(signal.PosA, signal.PosB, ParticleType.SwapSameEffect);
            }
            else
            {
                spawner.SpawnBetween(signal.PosA, signal.PosB, ParticleType.SwapEffect);
            }
        }

        private void OnMatchResolved(HeroMergedSignal signal)
        {
            spawner.SpawnDirectional(signal.Position, Vector3.up, ParticleType.MergeEffect);
        }
        void IDisposable.Dispose()
        {
            signalCenter.Unsubscribe<HeroSwappedSignal>(OnHeroSwapped);
            signalCenter.Unsubscribe<HeroMergedSignal>(OnMatchResolved);
        }

    }
}