using Game.Signal.Core;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using System;

namespace Game.SceneLayout
{
    public sealed class BackgroundScaler : IInitializable, IDisposable
    {
        [Inject] private readonly SignalCenter signalCenter;
        [Inject] private readonly Transform backgroundTransform;

        private readonly float initialCameraSize = 5f;

        void IInitializable.Initialize()
        {
            signalCenter.Subscribe<CameraSetupCompleteSignal>(UpdateBackground);
        }

        private void UpdateBackground(CameraSetupCompleteSignal _)
        {
            Camera cam = Camera.main;

            float currentCameraSize = cam.orthographicSize;
            float scaleMultiplier = currentCameraSize / initialCameraSize;
            float finalY = scaleMultiplier;
            float finalX = finalY;

            backgroundTransform.localScale = new Vector3(finalX, finalY, 1f);

            Vector3 pos = backgroundTransform.position;
            pos.y = cam.transform.position.y;
            backgroundTransform.position = pos;
        }
        void IDisposable.Dispose()
        {
            signalCenter.TryUnsubscribe<CameraSetupCompleteSignal>(UpdateBackground);
        }
    }
}