using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using Game.GridModule.Model;
using Game.HeroModule.View;
using Game.HeroModule.Model;
using UnityEngine;
using Game.HeroModule.Service;
using Game.Signal.Core;

namespace Game.HeroModule.Controller
{
    public sealed class HeroSwapProvider : ISwapProvider
    {
        private readonly HeroViewRegistery viewRegistery;
        private readonly SignalCenter signalCenter;

        public HeroSwapProvider(HeroViewRegistery rViewRegistery, SignalCenter rSignalCenter)
        {
            viewRegistery = rViewRegistery;
            signalCenter = rSignalCenter;
        }

        public bool CanSwap(Cell cellA, Cell cellB)
        {
            return Mathf.Abs(cellA.X - cellB.X) + Mathf.Abs(cellA.Y - cellB.Y) == 1;
        }

        public async UniTask<bool> TrySwapAsync(Cell cellA, Cell cellB, CancellationToken ct)
        {
            HeroView viewA = viewRegistery.GetView(cellA);
            HeroView viewB = viewRegistery.GetView(cellB);

            bool isSameHero = cellA.hero.Type == cellB.hero.Type && cellA.hero.Level == cellB.hero.Level;

            if (isSameHero)
            {
                await SwapSameHeroAsync(cellA, cellB, viewA, viewB, ct);
                return false;
            }

            signalCenter.Fire(new HeroSwappedSignal
            {
                PosA = cellA.WorldPosition,
                PosB = cellB.WorldPosition,
                IsSameType = false
            });

            Hero heroA = cellA.hero;
            Hero heroB = cellB.hero;

            cellA.SetHero(heroB);
            cellB.SetHero(heroA);

            viewRegistery.Register(cellA, viewB);
            viewRegistery.Register(cellB, viewA);

            Tween tweenA = viewA.transform.DOMove(cellB.WorldPosition, 0.2f).SetEase(Ease.OutBack);
            Tween tweenB = viewB.transform.DOMove(cellA.WorldPosition, 0.2f).SetEase(Ease.OutBack);

            await UniTask.WhenAll(tweenA.AsyncWaitForCompletion().AsUniTask(), tweenB.AsyncWaitForCompletion().AsUniTask());

            return true;
        }

        private async UniTask SwapSameHeroAsync(Cell cellA, Cell cellB, HeroView viewA, HeroView viewB, CancellationToken ct)
        {
            Transform transformA = viewA.transform;
            Transform transformB = viewB.transform;

            Vector3 posA = cellA.WorldPosition;
            Vector3 posB = cellB.WorldPosition;

            Vector3 targetForA = Vector3.Lerp(posA, posB, 0.75f);
            Vector3 targetForB = Vector3.Lerp(posB, posA, 0.75f);

            Sequence sequence = DOTween.Sequence();

            sequence.Append(transformA.DOMove(targetForA, 0.15f).SetEase(Ease.OutQuad));
            sequence.Join(transformB.DOMove(targetForB, 0.15f).SetEase(Ease.OutQuad));

            sequence.AppendCallback(() =>
            {
                signalCenter.Fire(new HeroSwappedSignal
                {
                    PosA = posA,
                    PosB = posB,
                    IsSameType = true
                });
            });

            sequence.Append(transformA.DOMove(posA, 0.15f).SetEase(Ease.InQuad));
            sequence.Join(transformB.DOMove(posB, 0.15f).SetEase(Ease.InQuad));

            await sequence.AsyncWaitForCompletion().AsUniTask();
        }

        public async UniTask PlayInvalidMoveAsync(Cell cell, Vector2Int direction, CancellationToken ct)
        {
            HeroView view = viewRegistery.GetView(cell);
            Transform transform = view.transform;

            Vector3 originalPos = transform.position;
            Vector3 originalScale = transform.localScale;

            Vector3 moveOffset = new Vector3(direction.x, direction.y, 0) * 0.25f;

            Vector3 squashScale;

            if (direction.x != 0)
            {
                squashScale = new Vector3(originalScale.x * 0.75f, originalScale.y * 1.2f, 1f);
            }
            else
            {
                squashScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 0.75f, 1f);
            }

            Sequence sequence = DOTween.Sequence();

            sequence.Append(transform.DOMove(originalPos + moveOffset, 0.1f).SetEase(Ease.OutQuad));
            sequence.Join(transform.DOScale(squashScale, 0.1f).SetEase(Ease.OutQuad));

            sequence.Append(transform.DOMove(originalPos, 0.12f).SetEase(Ease.OutBack));
            sequence.Join(transform.DOScale(originalScale, 0.12f).SetEase(Ease.OutBack));

            await sequence.AsyncWaitForCompletion().AsUniTask();
        }
    }
}
