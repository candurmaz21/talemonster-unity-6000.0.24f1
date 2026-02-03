using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.GoalModule.View
{
    public sealed class GoalView : MonoBehaviour
    {
        private const float FULL_ALPHA = 1f;
        private const float HALF_ALPHA = 0.5f;

        [SerializeField] private Image heroIcon;
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private GameObject checkMark;

        public void Setup(Sprite sprite, int requiredAmount)
        {
            heroIcon.sprite = sprite;
            countText.text = "x" + requiredAmount.ToString();
            checkMark.SetActive(false);
            SetAlpha(FULL_ALPHA);

            if (requiredAmount <= 0)
            {
                countText.text = "";
                checkMark.SetActive(true);
                SetAlpha(HALF_ALPHA);
            }
        }

        public void UpdateCount(int remaining)
        {
            if (remaining > 0)
            {
                countText.text = "x" + remaining.ToString();
                return;
            }

            countText.text = "";
            checkMark.SetActive(true);
            SetAlpha(HALF_ALPHA);
        }

        private void SetAlpha(float alpha)
        {
            Color color = heroIcon.color;
            color.a = alpha;
            heroIcon.color = color;
        }
    }
}