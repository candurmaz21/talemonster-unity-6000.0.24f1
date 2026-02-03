using System.Threading;
using Cysharp.Threading.Tasks;
using Game.GridModule;
using Game.GridModule.Model;
using UnityEngine;

namespace Game.HeroModule.Controller
{
    public interface ISwapProvider
    {
        UniTask<bool> TrySwapAsync(Cell cellA, Cell cellB, CancellationToken ct);
        UniTask PlayInvalidMoveAsync(Cell cell, Vector2Int direction, CancellationToken ct);
        bool CanSwap(Cell cellA, Cell cellB);
    }
}
