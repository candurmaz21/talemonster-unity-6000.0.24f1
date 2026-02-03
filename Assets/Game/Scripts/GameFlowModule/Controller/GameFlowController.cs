using System;
using System.Collections.Generic;
using Game.GoalModule.Model;
using Game.Signal.Core;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Game.LevelModule.Config;
using Game.HeroModule.Model;
using Game.LevelModule.Service;

namespace Game.GoalModule.Controller
{
    public sealed class GameFlowController : IInitializable, IStartable, IDisposable
    {
        [Inject] private readonly SignalCenter signalCenter;
        [Inject] private readonly LevelConfig levelConfig;
        [Inject] private readonly LevelProvider levelProvider;

        private List<GoalStatus> goals;
        private int currentProgress;
        private bool isGameEnded;

        void IInitializable.Initialize()
        {
            currentProgress = levelConfig.maxMoves;

            signalCenter.Subscribe<MoveUsedSignal>(OnMoveUsed);
            signalCenter.Subscribe<HeroGoalCollectedSignal>(OnHeroGoalCollected);
            signalCenter.Subscribe<TurnFinishedSignal>(OnTurnFinished);

            goals = new List<GoalStatus>();

            foreach (LevelGoalData goalData in levelConfig.goals)
            {
                goals.Add(new GoalStatus(
                    goalData.heroType,
                    goalData.targetRank,
                    goalData.requiredAmount
                ));
            }
        }

        void IStartable.Start()
        {
            signalCenter.Fire(new MoveCountUpdatedSignal(levelConfig.maxMoves));
            signalCenter.Fire(new GoalsInitializedSignal(goals));
        }

        public bool TryReserveHero(HeroType type, int level)
        {
            for (int i = 0; i < goals.Count; i++)
            {
                GoalStatus goal = goals[i];
                if (goal.Type == type && goal.Level == level && !goal.IsFullyReserved)
                {
                    goal.reservedAmount++;
                    goals[i] = goal;
                    return true;
                }
            }
            return false;
        }

        private void OnHeroGoalCollected(HeroGoalCollectedSignal signal)
        {
            for (int i = 0; i < goals.Count; i++)
            {
                GoalStatus goal = goals[i];
                if (goal.Type == signal.Type && goal.Level == signal.Level)
                {
                    goal.reservedAmount = Mathf.Max(0, goal.reservedAmount - 1);
                    goal.collectedAmount++;
                    goals[i] = goal;

                    signalCenter.Fire(new GoalProgressUpdatedSignal(i, goal));
                    break;
                }
            }
        }

        private void OnMoveUsed(MoveUsedSignal signal)
        {
            if (isGameEnded) return;

            currentProgress--;

            signalCenter.Fire(new MoveCountUpdatedSignal(currentProgress));
        }

        private void OnTurnFinished(TurnFinishedSignal signal)
        {
            if (isGameEnded) return;
            CheckWinCondition();

            if (!isGameEnded && currentProgress <= 0)
            {
                LoseGame();
            }
        }

        private void CheckWinCondition()
        {
            foreach (GoalStatus goal in goals)
            {
                if (!goal.IsCompleted)
                {
                    return;
                }
            }

            WinGame();
        }

        private void WinGame()
        {
            if (isGameEnded)
            {
                return;
            }

            levelProvider.IncrementLevel();

            isGameEnded = true;
            signalCenter.Fire(new GameEndedSignal(true));
        }

        private void LoseGame()
        {
            if (isGameEnded)
            {
                return;
            }

            isGameEnded = true;
            signalCenter.Fire(new GameEndedSignal(false));
        }

        void IDisposable.Dispose()
        {
            signalCenter.Unsubscribe<MoveUsedSignal>(OnMoveUsed);
            signalCenter.Unsubscribe<HeroGoalCollectedSignal>(OnHeroGoalCollected);
            signalCenter.Unsubscribe<TurnFinishedSignal>(OnTurnFinished);
        }
    }
}