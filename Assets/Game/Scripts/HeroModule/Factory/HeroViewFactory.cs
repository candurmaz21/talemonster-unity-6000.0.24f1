using Game.HeroModule.Config;
using Game.HeroModule.Model;
using Game.HeroModule.Service;
using Game.PoolSystem;
using UnityEngine;

namespace Game.HeroModule.View
{
    public sealed class HeroViewFactory
    {
        private readonly HeroPoolService pool;
        private readonly HeroConfig heroConfig;

        public HeroViewFactory(HeroPoolService rPool, HeroConfig rHeroConfig)
        {
            pool = rPool;
            heroConfig = rHeroConfig;
        }

        public HeroView Create(Hero hero, Transform parent)
        {
            HeroView view = pool.Get();
            view.transform.SetParent(parent, false);

            Sprite sprite = heroConfig.GetSprite(hero.Type, hero.Level);
            view.SetSprite(sprite);

            return view;
        }

        public void Release(HeroView view)
        {
            pool.Return(view);
        }
    }

}



