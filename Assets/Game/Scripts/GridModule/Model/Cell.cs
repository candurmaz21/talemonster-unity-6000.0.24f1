using UnityEngine;
using Game.HeroModule.View;
using Game.HeroModule.Model;

namespace Game.GridModule.Model
{
    public sealed class Cell
    {
        public readonly int X;
        public readonly int Y;
        public readonly Vector2 WorldPosition;

        public Hero hero;
        public bool isEmpty = true;

        public Cell(int x, int y, Vector2 rWorldPosition)
        {
            X = x;
            Y = y;
            WorldPosition = rWorldPosition;
        }

        public void SetHero(Hero rHero)
        {
            hero = rHero;
            isEmpty = false;
        }

        public void Clear()
        {
            isEmpty = true;
        }
    }
}
