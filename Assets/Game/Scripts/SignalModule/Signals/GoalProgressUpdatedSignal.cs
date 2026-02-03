using Game.GoalModule.Model;

namespace Game.Signal.Core
{
    public readonly struct GoalProgressUpdatedSignal
    {
        public readonly int GoalIndex;
        public readonly GoalStatus Status;

        public GoalProgressUpdatedSignal(int goalIndex, GoalStatus status)
        {
            GoalIndex = goalIndex;
            Status = status;
        }
    }
}