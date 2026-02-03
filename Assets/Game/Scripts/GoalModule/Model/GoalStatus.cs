using Game.HeroModule.Model;
using UnityEngine;

namespace Game.GoalModule.Model
{
    public struct GoalStatus
    {
        public readonly HeroType Type;
        public readonly int Level;
        public readonly int RequiredAmount;

        public int collectedAmount;
        public int reservedAmount;
        public bool IsFullyReserved => (collectedAmount + reservedAmount) >= RequiredAmount;
        public bool IsCompleted => collectedAmount >= RequiredAmount;

        public GoalStatus(HeroType type, int rank, int amount)
        {
            Type = type;
            Level = rank;
            RequiredAmount = amount;
            collectedAmount = 0;
            reservedAmount = 0;
        }
    }
}