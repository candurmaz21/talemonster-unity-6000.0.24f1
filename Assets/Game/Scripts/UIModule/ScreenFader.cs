using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace Game.UI.Common
{
    public sealed class ScreenFader : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;

        public async UniTask FadeInAsync(float duration = 0.5f)
        {
            canvasGroup.alpha = 1f;

            await canvasGroup.DOFade(0f, duration)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion()
                .AsUniTask();

            canvasGroup.blocksRaycasts = false;
        }
        public async UniTask FadeOutAsync(float duration = 0.5f)
        {
            canvasGroup.blocksRaycasts = true;

            await canvasGroup.DOFade(1f, duration)
                .SetEase(Ease.Linear)
                .AsyncWaitForCompletion()
                .AsUniTask();
        }
    }
}