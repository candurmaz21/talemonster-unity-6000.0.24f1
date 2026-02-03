using UnityEngine;
using TMPro;
using VContainer;
using Game.Signal.Core;

namespace Game.GoalModule.View
{
    public sealed class MoveView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI moveText;

        private SignalCenter signalCenter;

        [Inject]
        public void Construct(SignalCenter rSignalCenter)
        {
            signalCenter = rSignalCenter;
        }

        private void OnEnable()
        {
            signalCenter.Subscribe<MoveCountUpdatedSignal>(UpdateDisplay);
        }

        private void UpdateDisplay(MoveCountUpdatedSignal signal)
        {
            moveText.text = signal.RemainingMoves.ToString();
        }
        private void OnDisable()
        {
            signalCenter.Unsubscribe<MoveCountUpdatedSignal>(UpdateDisplay);
        }
    }
}