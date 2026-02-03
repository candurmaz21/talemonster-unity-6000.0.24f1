using UnityEngine;
using DG.Tweening;

namespace Game.HeroModule.View
{
    public sealed class HeroView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Pop Animation Values")]
        [SerializeField] private float squashMultiplier = 0.85f;
        [SerializeField] private float expandMultiplier = 1.2f;
        [SerializeField] private float squashTime = 0.07f;
        [SerializeField] private float expandTime = 0.12f;
        [SerializeField] private float originalScaleTime = 0.14f;

        private Vector3 originalScale;
        private Sequence popSequence;

        void Awake()
        {
            originalScale = transform.localScale;
            CreatePopSequence();
        }
        public void SetSprite(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
        }
        public void PlayPopAnimation()
        {
            popSequence.Restart();
        }
        public void ResetView()
        {
            transform.DOKill();
            transform.localScale = originalScale;
        }

        private void CreatePopSequence()
        {
            Vector3 squash = originalScale * squashMultiplier;
            Vector3 expand = originalScale * expandMultiplier;

            popSequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Pause();

            popSequence.Append(transform.DOScale(squash, squashTime).SetEase(Ease.OutQuad));
            popSequence.Append(transform.DOScale(expand, expandTime).SetEase(Ease.OutQuad));
            popSequence.Append(transform.DOScale(originalScale, originalScaleTime).SetEase(Ease.OutBack));
        }
    }
}
