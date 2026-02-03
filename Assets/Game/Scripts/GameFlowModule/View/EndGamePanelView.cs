using UnityEngine;
using UnityEngine.UI;
using Game.Signal.Core;
using VContainer;
using UnityEngine.SceneManagement;
using Game.UI.Common;
using Cysharp.Threading.Tasks;
using Game.LevelModule.Service;

namespace Game.GoalModule.View
{
    public sealed class EndGamePanelView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup panelGroup;
        [SerializeField] private Image resultImage;
        [SerializeField] private Sprite winSprite;
        [SerializeField] private Sprite loseSprite;
        [SerializeField] private Button actionButton;

        [Header("Transition")]
        [SerializeField] private ScreenFader screenFader;

        [Inject] private readonly SignalCenter signalCenter;

        private void OnEnable() => signalCenter.Subscribe<GameEndedSignal>(Show);

        private void Start()
        {
            screenFader.FadeInAsync().Forget();

            Hide();
        }

        private void Show(GameEndedSignal signal)
        {
            panelGroup.alpha = 1f;
            panelGroup.interactable = true;
            panelGroup.blocksRaycasts = true;

            resultImage.sprite = signal.IsWin ? winSprite : loseSprite;

            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(() => OnActionButtonClicked().Forget());
        }

        private async UniTaskVoid OnActionButtonClicked()
        {
            actionButton.interactable = false;
            await screenFader.FadeOutAsync(0.6f);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Hide()
        {
            panelGroup.alpha = 0f;
            panelGroup.interactable = false;
            panelGroup.blocksRaycasts = false;
        }

        private void OnDisable() => signalCenter.Unsubscribe<GameEndedSignal>(Show);
    }
}
