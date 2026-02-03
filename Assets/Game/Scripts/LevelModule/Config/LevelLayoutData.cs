using Game.HeroModule.Model;
using UnityEngine;

namespace Game.LevelModule.Config
{
    [CreateAssetMenu(menuName = "Config/LevelLayout")]
    public sealed class LevelLayoutData : ScriptableObject
    {
        public int width;
        public int height;
        public CellData[] cells;
    }

    [System.Serializable]
    public struct CellData
    {
        public int x;
        public int y;
        public HeroType heroType;
        public int level;
    }
}
