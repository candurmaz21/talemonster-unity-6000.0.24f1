using System.Collections.Generic;
using Game.GoalModule.Model;

namespace Game.Signal.Core
{
    public readonly struct GoalsInitializedSignal
    {
        public readonly IReadOnlyList<GoalStatus> Goals;

        public GoalsInitializedSignal(IReadOnlyList<GoalStatus> goals)
        {
            Goals = goals;
        }
    }
}