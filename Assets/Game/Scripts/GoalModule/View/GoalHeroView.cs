using UnityEngine;
using UnityEngine.UI;
using Game.HeroModule.Model;
using Game.HeroModule.Config;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;

namespace Game.GoalModule.View
{
    public sealed class GoalHeroView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image overlayImage;

        public RectTransform RectTransform => transform as RectTransform;

        private void OnDisable()
        {
            if (overlayImage != null)
            {
                Color c = overlayImage.color;
                c.a = 0f;
                overlayImage.color = c;
            }
        }

        public void Setup(HeroType type, int level, HeroConfig config)
        {
            icon.sprite = config.GetSprite(type, level);
            overlayImage.sprite = icon.sprite;
            ResetOverlay();
        }

        public async UniTask PlayOverlayFadeAsync(CancellationToken ct)
        {
            if (overlayImage == null)
            {
                return;
            }


            await overlayImage.DOFade(1f, 0.15f)
                .SetEase(Ease.OutCubic)
                .AsyncWaitForCompletion()
                .AsUniTask()
                .AttachExternalCancellation(ct);
        }

        public void ResetView()
        {
            ResetOverlay();
            gameObject.SetActive(false);
        }

        private void ResetOverlay()
        {
            if (overlayImage == null) return;

            Color tempColor = overlayImage.color;
            tempColor.a = 0f;
            overlayImage.color = tempColor;
        }
    }
}