using Game.LevelModule.Config;
using UnityEngine;

namespace Game.LevelModule.Service
{
    public sealed class LevelProvider
    {
        private const string LEVEL_INDEX_KEY = "CurrentLevelIndex";
        private readonly LevelsConfig levelsConfig;

        public int ConfigCount => levelsConfig.allLevels.Count;
        public int DisplayLevelNumber => CurrentLevelIndex + 1;
        public int LoopLevel => (CurrentLevelIndex % ConfigCount) + 1;

        public LevelProvider(LevelsConfig rLevelsConfig)
        {
            levelsConfig = rLevelsConfig;
        }

        public int CurrentLevelIndex
        {
            get => PlayerPrefs.GetInt(LEVEL_INDEX_KEY, 0);
            private set => PlayerPrefs.SetInt(LEVEL_INDEX_KEY, value);
        }

        public LevelConfig GetCurrentLevelConfig()
        {
            return levelsConfig.GetLevel(CurrentLevelIndex);
        }

        public void IncrementLevel()
        {
            CurrentLevelIndex++;
        }
    }
}