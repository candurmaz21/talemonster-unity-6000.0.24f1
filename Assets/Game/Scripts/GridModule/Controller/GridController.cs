using System.Collections.Generic;
using UnityEngine;
using Game.LevelModule.Config;
using Game.GridModule.Model;


namespace Game.GridModule.Controller
{
    public sealed class GridController : IGridProvider, IGridTransformProvider
    {
        public readonly float gridWorldWidth;
        public readonly float gridWorldHeight;
        public readonly int gridWidth;
        public readonly int gridHeight;

        private readonly List<Cell> cells;

        IReadOnlyList<Cell> IGridProvider.Cells => cells;
        int IGridProvider.GridWidth => gridWidth;
        int IGridProvider.GridHeight => gridHeight;

        float IGridTransformProvider.GridWorldWidth => gridWorldWidth;
        float IGridTransformProvider.GridWorldHeight => gridWorldHeight;

        public GridController(LevelConfig config)
        {
            gridWidth = config.gridWidth;
            gridHeight = config.gridHeight;

            if (config.levelLayoutData != null)
            {
                gridWidth = config.levelLayoutData.width;
                gridHeight = config.levelLayoutData.height;
            }

            float cellSize = config.cellSize;
            gridWorldWidth = gridWidth * cellSize;
            gridWorldHeight = gridHeight * cellSize;

            float offsetX = (gridWorldWidth - cellSize) * 0.5f;
            float offsetY = (gridWorldHeight - cellSize) * 0.5f;

            cells = new List<Cell>(gridWidth * gridHeight);

            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    float worldX = x * cellSize - offsetX;
                    float worldY = y * cellSize - offsetY;
                    cells.Add(new Cell(x, y, new Vector2(worldX, worldY)));
                }
            }
        }

        public bool IsValidCell(int x, int y)
        {
            return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
        }

        public Cell GetCell(int x, int y)
        {
            if (!IsValidCell(x, y))
            {
                return null;
            }

            return cells[y * gridWidth + x];
        }
    }
}