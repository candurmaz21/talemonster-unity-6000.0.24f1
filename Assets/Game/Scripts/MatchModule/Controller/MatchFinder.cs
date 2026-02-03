using System.Collections.Generic;
using Game.GridModule.Controller;
using Game.GridModule.Model;
using Game.MatchModule.Model;
using UnityEngine;

namespace Game.MatchModule.Controller
{
    public sealed class MatchFinder
    {

        private const int MAX_HERO_LEVEL = 3;
        private readonly IGridProvider gridProvider;

        private readonly List<MatchGroup> matchesCache = new List<MatchGroup>(16);
        private readonly List<Cell> currentDirMatchesCache = new List<Cell>(16);
        private readonly HashSet<Cell> visitedHorizontal = new HashSet<Cell>(64);
        private readonly HashSet<Cell> visitedVertical = new HashSet<Cell>(64);

        public MatchFinder(IGridProvider rGridProvider)
        {
            gridProvider = rGridProvider;
        }

        public List<MatchGroup> FindMatches()
        {
            matchesCache.Clear();
            visitedHorizontal.Clear();
            visitedVertical.Clear();

            for (int y = 0; y < gridProvider.GridHeight; y++)
            {
                for (int x = 0; x < gridProvider.GridWidth; x++)
                {
                    Cell cell = gridProvider.GetCell(x, y);
                    if (cell.isEmpty)
                    {
                        continue;
                    }

                    if (!visitedHorizontal.Contains(cell))
                    {
                        List<Cell> horizontalMatch = GetMatchInDirection(x, y, new Vector2Int(1, 0));
                        if (horizontalMatch.Count >= 3)
                        {
                            List<Cell> resultList = new List<Cell>(horizontalMatch);
                            matchesCache.Add(new MatchGroup { Cells = resultList, Level = cell.hero.Level });

                            foreach (Cell c in horizontalMatch)
                            {
                                visitedHorizontal.Add(c);
                            }

                        }
                    }

                    if (!visitedVertical.Contains(cell))
                    {
                        List<Cell> verticalMatch = GetMatchInDirection(x, y, new Vector2Int(0, 1));

                        if (verticalMatch.Count >= 3)
                        {
                            List<Cell> resultList = new List<Cell>(verticalMatch);
                            matchesCache.Add(new MatchGroup { Cells = resultList, Level = cell.hero.Level });

                            foreach (Cell c in verticalMatch)
                            {
                                visitedVertical.Add(c);
                            }
                        }
                    }
                }
            }
            return matchesCache;
        }

        private List<Cell> GetMatchInDirection(int startX, int startY, Vector2Int direction)
        {
            currentDirMatchesCache.Clear();
            Cell startCell = gridProvider.GetCell(startX, startY);
            currentDirMatchesCache.Add(startCell);

            int nextX = startX + direction.x;
            int nextY = startY + direction.y;

            while (gridProvider.IsValidCell(nextX, nextY))
            {
                Cell nextCell = gridProvider.GetCell(nextX, nextY);

                if (!nextCell.isEmpty && nextCell.hero.Type == startCell.hero.Type
                                && nextCell.hero.Level == startCell.hero.Level && nextCell.hero.Level < MAX_HERO_LEVEL)
                {
                    currentDirMatchesCache.Add(nextCell);

                    nextX += direction.x;
                    nextY += direction.y;
                }
                else
                {
                    break;
                }
            }
            return currentDirMatchesCache;
        }
    }
}