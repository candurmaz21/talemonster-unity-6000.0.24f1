using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.GridModule.Controller;
using Game.GridModule.Model;
using Game.HeroModule.Model;
using Game.HeroModule.View;
using Game.MatchModule.Model;
using Game.HeroModule.Service;
using UnityEngine;
using Game.MatchModule.Service;
using Game.Signal.Core;

namespace Game.MatchModule.Controller
{
    public sealed class HeroMatchController : IMatchController
    {
        private readonly HeroSpawner heroSpawnService;
        private readonly MatchAnimationService animationService;
        private readonly MatchFinder matchFinder;
        private readonly SignalCenter signalCenter;

        private readonly List<UniTask> mergeTasksCache = new List<UniTask>(16);
        private readonly List<Cell> priorityListCache = new List<Cell>(16);
        private readonly List<Cell> sortListCache = new List<Cell>(16);

        public HeroMatchController(HeroSpawner rHeroSpawnService, MatchAnimationService rAnimationService,
                                    MatchFinder rMatchFinder, SignalCenter rSignalCenter)
        {
            heroSpawnService = rHeroSpawnService;
            animationService = rAnimationService;
            matchFinder = rMatchFinder;
            signalCenter = rSignalCenter;
        }

        public async UniTask<bool> ProcessMatchesAsync(CancellationToken ct)
        {
            return await ProcessMatchesAsync(null, ct);
        }

        public async UniTask<bool> ProcessMatchesAsync(List<Cell> swappedCells, CancellationToken ct)
        {
            List<MatchGroup> matches = matchFinder.FindMatches();

            if (matches.Count == 0)
            {
                return false;
            }

            mergeTasksCache.Clear();

            foreach (MatchGroup group in matches)
            {
                mergeTasksCache.Add(ExecuteMerge(group, swappedCells, ct));
            }

            await UniTask.WhenAll(mergeTasksCache);

            await UniTask.Delay(50, cancellationToken: ct);

            return true;
        }

        private async UniTask ExecuteMerge(MatchGroup group, List<Cell> swappedCells, CancellationToken ct)
        {
            Cell animationTarget = GetSwappedCell(group, swappedCells) ?? group.Cells[group.Cells.Count / 2];

            await animationService.AnimateMergeAsync(group.Cells, animationTarget, ct);

            int spawnCount = group.Cells.Count - 2;
            int nextLevel = group.Level + 1;

            HeroType matchedHeroType = HeroType.Count;

            foreach (Cell cell in group.Cells)
            {
                if (cell.hero.Level < nextLevel)
                {
                    matchedHeroType = cell.hero.Type;
                    heroSpawnService.DespawnHeroFrom(cell);
                    cell.Clear();
                }
            }

            Cell swappedCell = GetSwappedCell(group, swappedCells);
            Cell center = group.Cells[group.Cells.Count / 2];

            priorityListCache.Clear();

            if (swappedCell != null) priorityListCache.Add(swappedCell);
            if (!priorityListCache.Contains(center)) priorityListCache.Add(center);

            SortByDistance(group.Cells, center);

            for (int i = 0; i < sortListCache.Count; i++)
            {
                Cell cell = sortListCache[i];
                if (!priorityListCache.Contains(cell))
                {
                    priorityListCache.Add(cell);
                }
            }

            int spawnedSoFar = 0;
            for (int i = 0; i < priorityListCache.Count; i++)
            {
                if (spawnedSoFar >= spawnCount) break;

                Cell target = priorityListCache[i];
                if (target.isEmpty)
                {
                    heroSpawnService.SpawnHeroAt(target, matchedHeroType, nextLevel);

                    signalCenter.Fire(new HeroCreatedSignal
                    {
                        Type = matchedHeroType,
                        Level = nextLevel,
                        WorldPosition = target.WorldPosition
                    });
                    signalCenter.Fire(new HeroMergedSignal
                    {
                        Position = target.WorldPosition
                    });

                    spawnedSoFar++;
                }
            }
        }

        private Cell GetSwappedCell(MatchGroup group, List<Cell> swappedCells)
        {
            if (swappedCells == null)
            {
                return null;
            }
            for (int i = 0; i < swappedCells.Count; i++)
            {
                if (group.Cells.Contains(swappedCells[i]))
                {
                    return swappedCells[i];
                }
            }

            return null;
        }

        private void SortByDistance(List<Cell> source, Cell reference)
        {
            sortListCache.Clear();
            sortListCache.AddRange(source);

            for (int i = 0; i < sortListCache.Count - 1; i++)
            {
                int bestIndex = i;
                int bestDist = GetDistance(sortListCache[i], reference);

                for (int j = i + 1; j < sortListCache.Count; j++)
                {
                    int dist = GetDistance(sortListCache[j], reference);
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        bestIndex = j;
                    }
                }

                if (bestIndex != i)
                {
                    Cell temp = sortListCache[i];
                    sortListCache[i] = sortListCache[bestIndex];
                    sortListCache[bestIndex] = temp;
                }
            }
        }

        private int GetDistance(Cell a, Cell b) => Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y);

    }
}