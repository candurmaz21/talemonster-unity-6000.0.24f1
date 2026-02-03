using TMPro;
using UnityEngine;
using VContainer;
using Game.LevelModule.Service;

namespace Game.LevelModule.View
{
    public sealed class LevelPanelView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private string prefix = "LEVEL ";

        [Inject] private readonly LevelProvider levelProvider;

        private void Start()
        {
            UpdateLevelDisplay();
        }

        public void UpdateLevelDisplay()
        {
            if (levelText == null) return;

            int displayLevel = levelProvider.LoopLevel;
            levelText.text = prefix + displayLevel;
        }
    }
}