using System.Collections.Generic;
using UnityEngine;

namespace Game.LevelModule.Config
{
    [CreateAssetMenu(menuName = "Config/LevelConfig")]
    public sealed class LevelConfig : ScriptableObject
    {
        [Header("Grid Layout")]
        public int gridWidth = 6;
        public int gridHeight = 6;
        public float cellSize = 1f;
        public float topBarWorldHeight = 1.5f;

        [Header("Game Rules")]
        public int maxMoves = 20;
        public List<LevelGoalData> goals;

        [Header("Optional (keep null for random)")]
        public LevelLayoutData levelLayoutData;

        private void OnValidate()
        {
            if (goals == null)
                goals = new List<LevelGoalData>();

            while (goals.Count < 3)
                goals.Add(default);

            if (goals.Count > 3)
                goals.RemoveRange(3, goals.Count - 3);
        }
    }
}
