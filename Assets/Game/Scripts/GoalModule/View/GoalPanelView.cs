using System.Collections.Generic;
using Game.GoalModule.Model;
using Game.GoalModule.Service;
using Game.HeroModule.Config;
using Game.HeroModule.Model;
using Game.Signal.Core;
using UnityEngine;
using VContainer;
using Game.Utils;

namespace Game.GoalModule.View
{
    public sealed class GoalPanelView : MonoBehaviour, IGoalUIProvider
    {
        [Inject] private readonly SignalCenter signalCenter;
        [Inject] private readonly HeroConfig heroConfig;
        [Inject] private readonly Canvas targetCanvas;

        [Header("Goal Settings")]
        [SerializeField] private GoalView goalPrefab;
        [SerializeField] private Transform goalContainer;

        private List<GoalView> createdViews = new List<GoalView>(8);
        private Dictionary<(HeroType, int), GoalView> goalMap = new Dictionary<(HeroType, int), GoalView>();

        private void OnEnable()
        {
            signalCenter.Subscribe<GoalsInitializedSignal>(PopulateGoals);
            signalCenter.Subscribe<GoalProgressUpdatedSignal>(OnGoalUpdated);
        }

        private void PopulateGoals(GoalsInitializedSignal signal)
        {
            foreach (Transform child in goalContainer)
            {
                Destroy(child.gameObject);
            }

            createdViews.Clear();
            goalMap.Clear();

            foreach (GoalStatus goal in signal.Goals)
            {
                GoalView view = Instantiate(goalPrefab, goalContainer);
                Sprite heroSprite = heroConfig.GetSprite(goal.Type, goal.Level);

                view.Setup(heroSprite, goal.RequiredAmount);

                createdViews.Add(view);
                goalMap[(goal.Type, goal.Level)] = view;
            }
        }

        private void OnGoalUpdated(GoalProgressUpdatedSignal signal)
        {
            if (signal.GoalIndex >= 0 && signal.GoalIndex < createdViews.Count)
            {
                int remaining = signal.Status.RequiredAmount - signal.Status.collectedAmount;
                createdViews[signal.GoalIndex].UpdateCount(remaining);
            }
        }

        public Vector2 GetGoalAnchoredPosition(HeroType type, int rank)
        {
            if (!goalMap.TryGetValue((type, rank), out GoalView view))
                return Vector2.zero;

            RectTransform goalRect = view.transform as RectTransform;

            return RectTransformUtil.ConvertToCanvasLocalPosition(goalRect, targetCanvas);
        }

        private void OnDisable()
        {
            signalCenter.Unsubscribe<GoalsInitializedSignal>(PopulateGoals);
            signalCenter.Unsubscribe<GoalProgressUpdatedSignal>(OnGoalUpdated);
        }
    }
}