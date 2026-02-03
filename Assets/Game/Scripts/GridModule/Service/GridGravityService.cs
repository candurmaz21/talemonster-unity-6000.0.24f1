using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.GridModule.Controller;
using Game.GridModule.Model;
using Game.HeroModule.Service;
using Game.HeroModule.View;
using UnityEngine;

namespace Game.GridModule.Service
{
    public sealed class GridGravityService : IGridGravityService
    {
        private readonly HeroViewRegistery viewRegistery;
        private readonly IGridProvider gridProvider;

        private readonly List<UniTask> moveTasksCache = new List<UniTask>(64);

        public GridGravityService(IGridProvider rGridProvider, HeroViewRegistery rViewRegistery)
        {
            viewRegistery = rViewRegistery;
            gridProvider = rGridProvider;
        }

        public async UniTask<bool> ApplyGravityAsync(CancellationToken ct)
        {
            bool hasMoved = false;
            moveTasksCache.Clear();

            for (int x = 0; x < gridProvider.GridWidth; x++)
            {
                for (int y = gridProvider.GridHeight - 1; y >= 0; y--)
                {
                    Cell currentCell = gridProvider.GetCell(x, y);
                    if (!currentCell.isEmpty)
                    {
                        continue;
                    }

                    Cell sourceCell = FindSourceCell(x, y);
                    if (sourceCell != null)
                    {
                        HeroView view = viewRegistery.GetView(sourceCell);

                        MoveHero(sourceCell, currentCell);

                        viewRegistery.Unregister(sourceCell);
                        viewRegistery.Register(currentCell, view);

                        moveTasksCache.Add(AnimateMove(currentCell, view, ct));
                        hasMoved = true;
                    }
                }
            }

            if (moveTasksCache.Count > 0)
            {
                await UniTask.WhenAll(moveTasksCache);
            }

            return hasMoved;
        }

        private Cell FindSourceCell(int x, int startY)
        {
            for (int y = startY - 1; y >= 0; y--)
            {
                Cell cell = gridProvider.GetCell(x, y);
                if (!cell.isEmpty)
                {
                    return cell;
                }
            }
            return null;
        }

        private void MoveHero(Cell source, Cell target)
        {
            target.SetHero(source.hero);
            source.Clear();
        }

        private async UniTask AnimateMove(Cell cell, HeroView view, CancellationToken ct)
        {
            await view.transform.DOMove(cell.WorldPosition, 0.15f)
                .SetEase(Ease.OutQuad)
                .AsyncWaitForCompletion()
                .AsUniTask();
        }
    }
}