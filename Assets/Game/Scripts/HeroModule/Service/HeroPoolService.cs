using System.Collections.Generic;
using UnityEngine;
using Game.HeroModule.View;
using Game.HeroModule.Model;
using Game.HeroModule.Config;
using Game.PoolSystem;
using Game.LevelModule.Config;

namespace Game.HeroModule.Service
{
    public sealed class HeroPoolService
    {
        private readonly GameObjectPool<HeroView> pool;

        public HeroPoolService(HeroConfig heroConfig, LevelConfig levelConfig, Transform heroPoolRoot)
        {
            int cellCount = levelConfig.gridWidth * levelConfig.gridHeight;
            int preWarmCount = cellCount + levelConfig.gridWidth;

            HeroView prefab = heroConfig.heroViewPrefab.GetComponent<HeroView>();

            pool = new GameObjectPool<HeroView>(
                prefab,
                heroPoolRoot,
                preWarmCount
            );
        }

        public HeroView Get()
        {
            return pool.Get();
        }

        public void Return(HeroView view)
        {
            pool.Return(view);

        }
    }
}
