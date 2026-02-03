using Game.HeroModule.Model;
using UnityEngine;

namespace Game.HeroModule.Config
{
    [CreateAssetMenu(menuName = "Config/HeroSO")]
    public sealed class HeroSO : ScriptableObject
    {
        public HeroType heroType;
        public HeroLevelSprite[] levelSprites;
        public Sprite GetSprite(int level)
        {
            foreach (HeroLevelSprite levelSprite in levelSprites)
            {
                if (levelSprite.level == level)
                {
                    return levelSprite.sprite;
                }
            }

            return null;
        }
    }
}