using Game.LevelModule.Config;
using Game.Signal.Core;
using UnityEngine;
using VContainer;
using Game.GridModule.Controller;
using VContainer.Unity;

namespace Game.SceneLayout
{
    public sealed class CameraFramer : IStartable
    {
        const float VERTICAL_SAFE_RATIO = 0.5f;
        const float HORIZONTAL_SAFE_RATIO = 0.9f;
        const float CAMERA_Z = -10f;
        const float HALF = 0.5f;

        [Inject] readonly IGridTransformProvider gridTransformProvider;
        [Inject] readonly LevelConfig config;
        [Inject] readonly SignalCenter signalCenter;

        void IStartable.Start()
        {
            Camera cam = Camera.main;

            float gridH = gridTransformProvider.GridWorldHeight;
            float gridW = gridTransformProvider.GridWorldWidth;

            float sizeBasedOnHeight = (gridH * HALF) / VERTICAL_SAFE_RATIO;
            float sizeBasedOnWidth = (gridW / cam.aspect * HALF) / HORIZONTAL_SAFE_RATIO;

            cam.orthographicSize = Mathf.Max(sizeBasedOnHeight, sizeBasedOnWidth);
            cam.transform.position = new Vector3(0f, 0f, CAMERA_Z);

            signalCenter.Fire(new CameraSetupCompleteSignal());
        }
    }
}