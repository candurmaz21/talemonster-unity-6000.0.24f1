using UnityEngine;
using Game.HeroModule.Model;

namespace Game.HeroModule.Config
{
    [CreateAssetMenu(menuName = "Config/HeroConfig")]
    public sealed class HeroConfig : ScriptableObject
    {
        public HeroSO[] heroes;
        public GameObject heroViewPrefab;

        public Sprite GetSprite(HeroType type, int level)
        {
            foreach (HeroSO hero in heroes)
            {
                if (hero.heroType == type)
                    return hero.GetSprite(level);
            }
            return null;
        }
    }
}