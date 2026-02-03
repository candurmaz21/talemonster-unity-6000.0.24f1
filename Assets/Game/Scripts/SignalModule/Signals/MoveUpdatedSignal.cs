namespace Game.Signal.Core
{
    public struct MoveCountUpdatedSignal
    {
        public readonly int RemainingMoves;
        public MoveCountUpdatedSignal(int remainingMoves) => RemainingMoves = remainingMoves;
    }
}