using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.BoardModule.Model;
using Game.GoalModule.Controller;
using Game.GridModule.Model;
using Game.GridModule.Service;
using Game.HeroModule.Controller;
using Game.MatchModule.Controller;
using Game.Signal.Core;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.BoardModule.Controller
{
    public sealed class BoardController : IInitializable, IDisposable
    {
        [Inject] private readonly ISwapProvider swapProvider;
        [Inject] private readonly IMatchController matchProcessor;
        [Inject] private readonly IGridGravityService gridGravityService;
        [Inject] private readonly IBoardFillController refillService;
        [Inject] private readonly GoalCollectionController goalCollectionController;
        [Inject] private readonly SignalCenter signalCenter;

        private BoardState state = BoardState.Idle;
        private CancellationTokenSource cts;
        private List<Cell> swappedCellsCache = new List<Cell>(2);

        void IInitializable.Initialize()
        {
            cts = new CancellationTokenSource();
            signalCenter.Subscribe<GameEndedSignal>(OnGameEnded);
        }
        public async UniTask<bool> TrySwap(Cell cellA, Cell cellB)
        {
            if (state != BoardState.Idle || !swapProvider.CanSwap(cellA, cellB))
            {
                return false;
            }

            SetState(BoardState.Swapping);

            bool swapSuccess = await swapProvider.TrySwapAsync(cellA, cellB, cts.Token);

            if (!swapSuccess)
            {
                SetState(BoardState.Idle);
                return false;
            }

            signalCenter.Fire(new MoveUsedSignal());

            swappedCellsCache.Clear();
            swappedCellsCache.Add(cellA);
            swappedCellsCache.Add(cellB);

            List<Cell> checkCells = swappedCellsCache;
            bool needsCycle = true;

            while (needsCycle)
            {
                SetState(BoardState.Resolving);
                bool matched = await matchProcessor.ProcessMatchesAsync(checkCells, cts.Token);

                SetState(BoardState.Gravity);
                bool moved = await gridGravityService.ApplyGravityAsync(cts.Token);

                SetState(BoardState.Refill);
                bool refilled = await refillService.RefillAsync(cts.Token);

                needsCycle = matched || moved || refilled;
                checkCells = null;
            }

            await UniTask.WaitUntil(() => !goalCollectionController.IsAnimating, cancellationToken: cts.Token);

            signalCenter.Fire(new TurnFinishedSignal());

            if (state != BoardState.GameEnded)
            {
                SetState(BoardState.Idle);
            }

            return true;
        }

        public async UniTask PlayInvalidMoveAnimation(Cell cell, Vector2Int direction)
        {
            if (state != BoardState.Idle) return;

            SetState(BoardState.Swapping);
            try
            {
                await swapProvider.PlayInvalidMoveAsync(cell, direction, cts.Token);
            }
            finally
            {
                SetState(BoardState.Idle);
            }
        }

        private void OnGameEnded(GameEndedSignal signal) => SetState(BoardState.GameEnded);

        private void SetState(BoardState newState) => state = newState;

        void IDisposable.Dispose()
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
            signalCenter.Unsubscribe<GameEndedSignal>(OnGameEnded);
        }
    }
}