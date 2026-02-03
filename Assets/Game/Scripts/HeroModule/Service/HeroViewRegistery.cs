using System.Collections.Generic;
using Game.GridModule.Model;
using Game.HeroModule.View;
using UnityEngine;

namespace Game.HeroModule.Service
{
    public sealed class HeroViewRegistery
    {
        private readonly Dictionary<Cell, HeroView> map = new();

        public void Register(Cell cell, HeroView heroView) => map[cell] = heroView;
        public void Unregister(Cell cell) => map.Remove(cell);

        public HeroView GetView(Cell cell) => map.TryGetValue(cell, out HeroView view) ? view : null;
    }
}
