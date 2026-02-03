using Game.HeroModule.Model;
using UnityEngine;

namespace Game.GoalModule.Service
{
    public interface IGoalUIProvider
    {
        Vector2 GetGoalAnchoredPosition(HeroType type, int rank);
    }
}