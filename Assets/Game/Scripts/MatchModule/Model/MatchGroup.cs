using System.Collections.Generic;
using Game.GridModule.Model;

namespace Game.MatchModule.Model
{
    public sealed class MatchGroup
    {
        public List<Cell> Cells = new List<Cell>();

        public int Level;
    }
}