using UnityEngine;
using Game.HeroModule.Model;
using Game.HeroModule.View;
using Game.GridModule.Model;

namespace Game.HeroModule.Service
{
    public sealed class HeroSpawner
    {
        private readonly HeroViewFactory heroFactory;
        private readonly Transform heroPoolRoot;
        private readonly HeroViewRegistery viewRegistery;

        public HeroSpawner(HeroViewFactory rHeroFactory, Transform rHeroPoolRoot, HeroViewRegistery rViewRegistery)
        {
            heroFactory = rHeroFactory;
            heroPoolRoot = rHeroPoolRoot;
            viewRegistery = rViewRegistery;
        }

        public void SpawnHeroAt(Cell cell, HeroType type, int level)
        {
            Hero newHero = new Hero(type, level);
            HeroView newView = heroFactory.Create(newHero, heroPoolRoot);

            newView.transform.position = cell.WorldPosition;

            cell.SetHero(newHero);
            viewRegistery.Register(cell, newView);

            if (level > 0)
            {
                newView.PlayPopAnimation();
            }
        }

        public void DespawnHeroFrom(Cell cell)
        {
            if (cell.isEmpty)
            {
                return;
            }
            HeroView view = viewRegistery.GetView(cell);

            if (view != null)
            {
                view.ResetView();
                heroFactory.Release(view);
                viewRegistery.Unregister(cell);
            }
            cell.Clear();
        }
    }
}