using System.Collections.Generic;
using Game.LevelModule.Config;
using UnityEngine;

namespace Game.LevelModule.Config
{
    [CreateAssetMenu(menuName = "Config/AllLevelsConfig")]
    public sealed class LevelsConfig : ScriptableObject
    {
        public List<LevelConfig> allLevels;

        public LevelConfig GetLevel(int index)
        {
            if (allLevels == null || allLevels.Count == 0)
            {
                return null;
            }
            int loopIndex = index % allLevels.Count;
            
            return allLevels[loopIndex];
        }
    }
}