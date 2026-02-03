using System.Threading;
using Cysharp.Threading.Tasks;
using Game.GoalModule.Controller;
using Game.HeroModule.Model;
using Game.Signal.Core;
using UnityEngine;
using VContainer.Unity;
using System;
using Game.GoalModule.View;
using Game.GoalModule.Service;
using DG.Tweening;

namespace Game.GoalModule.Controller
{
    public sealed class GoalCollectionController : IInitializable, IDisposable
    {
        private readonly GameFlowController gameFlowController;
        private readonly SignalCenter signalCenter;
        private readonly IGoalCollectAnimator animator;

        private CancellationTokenSource cts;

        private int activeAnimationsCount = 0;
        public bool IsAnimating => activeAnimationsCount > 0;

        public GoalCollectionController(GameFlowController rGameFlowController, SignalCenter rSignalCenter, IGoalCollectAnimator rAnimator)
        {
            gameFlowController = rGameFlowController;
            signalCenter = rSignalCenter;
            animator = rAnimator;
        }

        void IInitializable.Initialize()
        {
            cts = new CancellationTokenSource();
            signalCenter.Subscribe<HeroCreatedSignal>(OnHeroCreated);
        }

        private void OnHeroCreated(HeroCreatedSignal signal)
        {
            ProcessHeroGoalAsync(signal.Type, signal.Level, signal.WorldPosition, cts.Token).Forget();
        }

        private async UniTask ProcessHeroGoalAsync(HeroType type, int level, Vector3 startWorldPos, CancellationToken ct)
        {
            if (!gameFlowController.TryReserveHero(type, level))
            {
                return;
            }

            activeAnimationsCount++;

            try
            {
                await animator.Play(type, level, startWorldPos, ct);
                signalCenter.Fire(new HeroGoalCollectedSignal(type, level));
            }
            finally
            {
                activeAnimationsCount--;
            }
        }

        void IDisposable.Dispose()
        {
            signalCenter.Unsubscribe<HeroCreatedSignal>(OnHeroCreated);
            cts?.Cancel();
            cts?.Dispose();
        }
    }
}