namespace Game.BoardModule.Model
{
    public enum BoardState
    {
        Idle,
        Swapping,
        Resolving,
        Gravity,
        Refill,
        GoalCheck,
        GameEnded,
        Locked = 99
    }
}