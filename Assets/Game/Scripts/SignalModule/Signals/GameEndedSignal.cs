namespace Game.Signal.Core
{
    public readonly struct GameEndedSignal
    {
        public readonly bool IsWin;

        public GameEndedSignal(bool isWin)
        {
            IsWin = isWin;
        }
    }
}