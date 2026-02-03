using Game.HeroModule.Model;

namespace Game.Signal.Core
{
    public readonly struct HeroGoalCollectedSignal
    {
        public readonly HeroType Type;
        public readonly int Level;

        public HeroGoalCollectedSignal(HeroModule.Model.HeroType type, int level)
        {
            Type = type;
            Level = level;
        }
    }
}