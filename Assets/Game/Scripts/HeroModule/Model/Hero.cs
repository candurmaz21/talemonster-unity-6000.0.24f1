namespace Game.HeroModule.Model
{
    public struct Hero
    {
        public HeroType Type { get; }
        public int Level { get; }
        public Hero(HeroType type, int level)
        {
            Type = type;
            Level = level;
        }
    }
}