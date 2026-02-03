using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.GridModule.Controller;
using Game.GridModule.Model;
using Game.HeroModule.Model;
using Game.HeroModule.Service;
using Game.LevelModule.Config;
using UnityEngine;
using VContainer.Unity;
using VContainer;
using Game.HeroModule.View;

namespace Game.BoardModule.Controller
{
    public sealed class BoardFillController : IBoardFillController, IStartable
    {
        private readonly HeroSpawner heroSpawner;
        private readonly LevelConfig levelConfig;
        private readonly HeroViewRegistery viewRegistery;
        private readonly HeroType[] allTypes;
        private readonly IGridProvider gridProvider;

        private readonly List<UniTask> tasksCache = new List<UniTask>(64);
        private readonly List<HeroType> candidatesCache = new List<HeroType>(8);

        [Inject]
        public BoardFillController(IGridProvider rGridProvider, HeroSpawner rHeroSpawner,
                                    LevelConfig rLevelConfig, HeroViewRegistery rViewRegistery)
        {
            gridProvider = rGridProvider;
            heroSpawner = rHeroSpawner;
            levelConfig = rLevelConfig;
            viewRegistery = rViewRegistery;

            allTypes = new HeroType[(int)HeroType.Count];
            for (int i = 0; i < allTypes.Length; i++)
            {
                allTypes[i] = (HeroType)i;
            }
        }

        void IStartable.Start()
        {
            InitialFill(levelConfig.levelLayoutData);
        }

        public void InitialFill(LevelLayoutData boardLayout = null)
        {
            if (boardLayout != null)
            {
                PopulateFromLayout(boardLayout);
                return;
            }

            for (int y = 0; y < gridProvider.GridHeight; y++)
            {
                for (int x = 0; x < gridProvider.GridWidth; x++)
                {
                    Cell cell = gridProvider.GetCell(x, y);
                    HeroType type = GetRandomType(true, cell);
                    heroSpawner.SpawnHeroAt(cell, type, 0);
                }
            }
        }

        private void PopulateFromLayout(LevelLayoutData layout)
        {
            foreach (CellData data in layout.cells)
            {
                Cell cell = gridProvider.GetCell(data.x, data.y);
                heroSpawner.SpawnHeroAt(cell, data.heroType, data.level);
            }
        }

        public async UniTask<bool> RefillAsync(CancellationToken ct)
        {
            bool spawnedAny = false;
            tasksCache.Clear();

            for (int x = 0; x < gridProvider.GridWidth; x++)
            {
                for (int y = 0; y < gridProvider.GridHeight; y++)
                {
                    Cell cell = gridProvider.GetCell(x, y);
                    if (cell.isEmpty)
                    {
                        HeroType type = GetRandomType(false);
                        heroSpawner.SpawnHeroAt(cell, type, 0);

                        tasksCache.Add(PlaySpawnAnimation(cell, ct));
                        spawnedAny = true;
                    }
                }
            }

            if (tasksCache.Count > 0)
            {
                await UniTask.WhenAll(tasksCache);
            }

            return spawnedAny;
        }

        private HeroType GetRandomType(bool preventMatch, Cell cell = null)
        {
            if (!preventMatch)
            {
                return allTypes[UnityEngine.Random.Range(0, allTypes.Length)];
            }

            candidatesCache.Clear();
            candidatesCache.AddRange(allTypes);

            RemoveHorizontalMatch(cell, candidatesCache);
            RemoveVerticalMatch(cell, candidatesCache);

            return candidatesCache[UnityEngine.Random.Range(0, candidatesCache.Count)];
        }

        private void RemoveHorizontalMatch(Cell cell, List<HeroType> candidates)
        {
            if (cell.X < 2) return;

            Hero left = gridProvider.GetCell(cell.X - 1, cell.Y).hero;
            Hero doubleLeft = gridProvider.GetCell(cell.X - 2, cell.Y).hero;

            if (left.Type == doubleLeft.Type)
            {
                candidates.Remove(left.Type);
            }
        }

        private void RemoveVerticalMatch(Cell cell, List<HeroType> candidates)
        {
            if (cell.Y < 2) return;

            Hero down = gridProvider.GetCell(cell.X, cell.Y - 1).hero;
            Hero doubleDown = gridProvider.GetCell(cell.X, cell.Y - 2).hero;

            if (down.Type == doubleDown.Type)
            {
                candidates.Remove(down.Type);
            }
        }

        private async UniTask PlaySpawnAnimation(Cell cell, CancellationToken ct)
        {
            HeroView view = viewRegistery.GetView(cell);
            Vector3 targetPos = cell.WorldPosition;
            view.transform.position = targetPos + Vector3.down * 2f;

            await view.transform.DOMove(targetPos, 0.25f)
                .SetEase(Ease.OutBack)
                .AsyncWaitForCompletion()
                .AsUniTask();
        }
    }
}