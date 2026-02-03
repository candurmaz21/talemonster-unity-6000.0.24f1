using Game.GridModule.Controller;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.SceneLayout
{
    public sealed class GridBackgroundSizer : IStartable
    {
        [Inject] private readonly IGridTransformProvider gridTransformProvider;
        [Inject] private readonly SpriteRenderer backgroundRenderer;
        private const float padding = 0.5f;

        void IStartable.Start()
        {
            Vector2 size = backgroundRenderer.size;
            size.x = gridTransformProvider.GridWorldWidth + padding;
            size.y = gridTransformProvider.GridWorldHeight + padding;
            backgroundRenderer.size = size;
        }
    }
}