using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.GridModule.Model;
using Game.HeroModule.Service;
using Game.HeroModule.View;
using UnityEngine;

namespace Game.MatchModule.Service
{
    public sealed class MatchAnimationService
    {
        private readonly List<UniTask> tasksCache = new List<UniTask>(16);
        private readonly HeroViewRegistery viewRegistery;
        public MatchAnimationService(HeroViewRegistery rViewRegistery)
        {
            viewRegistery = rViewRegistery;
        }
        public async UniTask AnimateMergeAsync(List<Cell> cells, Cell targetCell, CancellationToken ct)
        {
            tasksCache.Clear();
            Vector3 targetPos = targetCell.WorldPosition;

            foreach (Cell cell in cells)
            {
                HeroView view = viewRegistery.GetView(cell);

                if (cell == targetCell || view == null) continue;

                Tween moveTween = view.transform
                    .DOMove(targetPos, 0.2f)
                    .SetEase(Ease.InBack);

                tasksCache.Add(moveTween.AsyncWaitForCompletion().AsUniTask());
            }

            if (tasksCache.Count > 0)
            {
                await UniTask.WhenAll(tasksCache);
            }
        }
    }
}