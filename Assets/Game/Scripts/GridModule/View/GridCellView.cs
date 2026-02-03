using Game.HeroModule.View;
using UnityEngine;
using VContainer;
using Game.GridModule.Controller;
using Game.GridModule.Model;
using Game.GoalModule.Controller;
using Cysharp.Threading.Tasks;
using Game.BoardModule.Controller;

namespace Game.GridModule.View
{
    public sealed class GridCellView : MonoBehaviour
    {
        private const float SWIPE_THRESHOLD = 0.5f;

        [Inject] private IGridProvider gridProvider;
        [Inject] private readonly BoardController boardManager;
        [Inject] private Camera mainCamera;

        public Cell Cell;
        private bool isDragging;
        private Vector2 dragStartPosition;

        public void Init(Cell cell)
        {
            Cell = cell;
        }

        private void OnMouseDown()
        {
            isDragging = true;
            dragStartPosition = GetWorldPosition(Input.mousePosition);
        }

        private void OnMouseDrag()
        {
            if (!isDragging)
            {
                return;
            }

            Vector2 delta = GetDragDelta();
            if (!IsSwiped(delta))
            {
                return;
            }

            TryHandleSwipe(delta);
            isDragging = false;
        }

        private void OnMouseUp()
        {
            isDragging = false;
        }

        private Vector2 GetDragDelta()
        {
            Vector2 currentPosition = GetWorldPosition(Input.mousePosition);
            return currentPosition - dragStartPosition;
        }

        private bool IsSwiped(Vector2 delta)
        {
            return delta.sqrMagnitude >= SWIPE_THRESHOLD * SWIPE_THRESHOLD;
        }
        private void TryHandleSwipe(Vector2 delta)
        {
            Vector2Int direction = GetSwipeDirection(delta);

            if (!TryGetTargetCell(direction, out Cell targetCell))
            {
                OnInvalidSwipe(direction);
                {
                    return;
                }
            }

            OnValidSwipe(targetCell.X, targetCell.Y).Forget();
        }
        private bool TryGetTargetCell(Vector2Int direction, out Cell targetCell)
        {
            int targetX = Cell.X + direction.x;
            int targetY = Cell.Y + direction.y;

            if (!gridProvider.IsValidCell(targetX, targetY))
            {
                targetCell = null;
                return false;
            }

            targetCell = gridProvider.GetCell(targetX, targetY);
            return true;
        }

        private Vector2Int GetSwipeDirection(Vector2 delta)
        {
            bool isHorizontal = Mathf.Abs(delta.x) > Mathf.Abs(delta.y);

            if (isHorizontal)
            {
                return new Vector2Int(delta.x > 0 ? 1 : -1, 0);
            }
            return new Vector2Int(0, delta.y > 0 ? 1 : -1);
        }

        private Vector2 GetWorldPosition(Vector3 screenPosition)
        {
            return mainCamera.ScreenToWorldPoint(screenPosition);
        }

        private async UniTaskVoid OnValidSwipe(int targetX, int targetY)
        {
            Cell targetCell = gridProvider.GetCell(targetX, targetY);
            bool isSwapSuccessful = await boardManager.TrySwap(Cell, targetCell);

#if UNITY_EDITOR
            if (isSwapSuccessful)
            {
                //Debug.Log("Swapped: (" + Cell.X + ", " + Cell.Y + ") -> (" + targetX + ", " + targetY + ")");
            }
#endif
        }

        private void OnInvalidSwipe(Vector2Int direction)
        {
            boardManager.PlayInvalidMoveAnimation(Cell, direction).Forget();
#if UNITY_EDITOR
            //Debug.Log("Out of bounds");
#endif
        }
    }
}