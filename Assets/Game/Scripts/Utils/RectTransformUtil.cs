using UnityEngine;


namespace Game.Utils
{
    public static class RectTransformUtil
    {
        public static Vector2 ConvertToCanvasLocalPosition(RectTransform sourceRect, Canvas targetCanvas)
        {
            Camera cam = targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : targetCanvas.worldCamera;
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, sourceRect.position);

            RectTransform canvasRect = targetCanvas.transform as RectTransform;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, cam, out Vector2 localPoint);

            return localPoint;
        }
    }
}
