

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.GoalModule.Service;
using Game.GoalModule.View;
using Game.HeroModule.Model;
using UnityEngine;
using Game.Utils.Math;

public sealed class GoalCollectAnimator : IGoalCollectAnimator
{
    private readonly GoalPoolService pool;
    private readonly IGoalUIProvider uiProvider;
    private readonly Canvas canvas;
    private readonly Camera worldCamera;

    public GoalCollectAnimator(GoalPoolService rPool, IGoalUIProvider rUiProvider, Canvas rCanvas, Camera rWorldCamera)
    {
        pool = rPool;
        uiProvider = rUiProvider;
        canvas = rCanvas;
        worldCamera = rWorldCamera;
    }
    public async UniTask Play(HeroType type, int level, Vector3 worldStartPos, CancellationToken ct)
    {
        GoalHeroView view = pool.Get(type, level);
        RectTransform rect = view.RectTransform;

        Vector2 startPos = WorldToCanvas(worldStartPos);
        Vector2 endPos = uiProvider.GetGoalAnchoredPosition(type, level);
        Vector2 controlPos = BezierUtil.GetParabolaControlPoint(startPos, endPos);

        rect.anchoredPosition = startPos;
        view.gameObject.SetActive(true);

        float t = 0f;
        float duration = 0.5f;

        await DOTween.To(
            () =>
            t, value =>
            {
                t = value;

                float heightT = DOVirtual.EasedValue(0f, 1f, t, Ease.OutQuad);
                Vector2 curvedControl = Vector2.Lerp(startPos, controlPos, heightT);

                rect.anchoredPosition = BezierUtil.Quadratic(startPos, curvedControl, endPos, t);
            }, 1f, duration)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion()
                .AsUniTask()
                .AttachExternalCancellation(ct);

        await view.PlayOverlayFadeAsync(ct);
        pool.Return(view);
    }

    private Vector2 WorldToCanvas(Vector3 worldPos)
    {
        RectTransform canvasRect = canvas.transform as RectTransform;

        Vector2 screen = RectTransformUtility.WorldToScreenPoint(worldCamera, worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screen,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera, out Vector2 local);

        return local;
    }
}
