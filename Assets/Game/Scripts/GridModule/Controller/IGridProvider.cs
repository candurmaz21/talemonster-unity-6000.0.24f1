
using Game.GridModule.Model;
using System.Collections.Generic;

namespace Game.GridModule.Controller
{
    public interface IGridProvider
    {
        IReadOnlyList<Cell> Cells { get; }
        int GridWidth { get; }
        int GridHeight { get; }
        Cell GetCell(int x, int y);
        bool IsValidCell(int x, int y);
    }
}