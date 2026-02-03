using Game.GoalModule.View;
using Game.HeroModule.Config;
using Game.HeroModule.Model;
using Game.PoolSystem;
using UnityEngine;

namespace Game.GoalModule.Service
{
    public sealed class GoalPoolService
    {
        private readonly GameObjectPool<GoalHeroView> pool;
        private readonly HeroConfig heroConfig;

        public GoalPoolService(GoalHeroView rPrefab, Transform rPoolRoot, HeroConfig rHeroConfig)
        {
            heroConfig = rHeroConfig;
            pool = new GameObjectPool<GoalHeroView>(rPrefab, rPoolRoot, 5);
        }

        public GoalHeroView Get(HeroType type, int level)
        {
            GoalHeroView view = pool.Get();
            view.Setup(type, level, heroConfig);
            return view;
        }

        public void Return(GoalHeroView view)
        {
            view.ResetView();
            pool.Return(view);
        }
    }
}