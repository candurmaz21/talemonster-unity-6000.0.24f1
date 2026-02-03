using Game.HeroModule.Model;

namespace Game.LevelModule.Config
{
    [System.Serializable]
    public struct LevelGoalData
    {
        public HeroType heroType;
        public int targetRank;
        public int requiredAmount;
    }
}