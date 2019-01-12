using System;

namespace MadEngine
{
    public struct BoardInfo
    {
        public BoardInfo(int boardLength = 40)
        {
            Length = boardLength;
        }

        // length of the shared track around the board (excluding the collision-free end-zones leading to the goal).
        public int Length { get; }

        // length of the collision-free end-zones leading to the goal.
        private int EndZoneLength
            => Length / 8;

        // length of the board + length of end-zone.
        public int GoalDistance
            => Length + EndZoneLength;

        // where on the board does player X start (when they move a piece of from their base).
        public int StartPosition(int player)
            => (Length / 4) * player;

        // checks if the distance of a piece corresponds to one of the collision-free end-zones.
        public bool IsInEndZone(int pieceDistance)
            => pieceDistance > Length && pieceDistance < GoalDistance;
    }
}
