using System;

namespace Ludo.GameLogic
{
    public struct Rules
    {
        public Rules(bool allowGoalBouncing, bool allowBaseExitOnRoll1)
        {
            AllowGoalBouncing = allowGoalBouncing;
            AllowBaseExitOnRoll1 = allowBaseExitOnRoll1;
        }

        // Allow rolls that would move a piece beyond the goal to bounce that piece against the goal.
        public bool AllowGoalBouncing { get; }

        // Allow exiting the base with a roll of 1 (in addition to rolls of 6).
        public bool AllowBaseExitOnRoll1 { get; }
    }
}
