using System;

namespace MadEngine
{
    public struct PieceInfo
    {
        // how far has this piece moved (from its base, or zero if it is currently in the base).
        public byte CurrentDistance { get; }

        // where is this piece on the board? (assuming it is not in base or goal, otherwise -1)
        public sbyte CurrentPosition { get; }

        // if the piece is moved, where will it be after it has moved? (assuming it can, otherwise == CurrentPosition)
        public sbyte MovedPosition { get; }

        // can it move? (Engine sets this based on board state / all the rules of the game)
        public bool CanMove => CurrentPosition != MovedPosition;

        // if this piece can move, and it is moved, will it collide with another piece? (otherwise NULL)
        public CollisionInfo? MovedCollision { get; }

        // is this piece in its base?
        public bool IsInBase => CurrentDistance == 0;

        // has this piece left the game board? (reached the goal)
        public bool IsInGoal => !IsInBase && CurrentPosition == -1;

        #region ### ctor ###
        // constructor for piece that cannot move.
        public PieceInfo(int currentDistance, int currentPosition)
        {
            CurrentDistance = (byte)currentDistance;
            CurrentPosition = (sbyte)currentPosition;
            MovedPosition = (sbyte)currentPosition;
            MovedCollision = null;
        }

        // constructor for piece that can move without colliding.
        public PieceInfo(int currentDistance, int currentPosition, int movedPosition)
        {
            CurrentDistance = (byte)currentDistance;
            CurrentPosition = (sbyte)currentPosition;
            MovedPosition = (sbyte)movedPosition;
            MovedCollision = null;
        }

        // constructor for piece that can move AND collides with another piece if it is moved.
        public PieceInfo(int currentDistance, int currentPosition, int movedPosition, CollisionInfo movedCollision)
        {
            CurrentDistance = (byte)currentDistance;
            CurrentPosition = (sbyte)currentPosition;
            MovedPosition = (sbyte)movedPosition;
            MovedCollision = movedCollision;
        }
        #endregion
    }
}
