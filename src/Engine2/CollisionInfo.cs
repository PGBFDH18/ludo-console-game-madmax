using System;

namespace MadEngine
{
    public struct CollisionInfo
    {
        // who does the colliding piece belong to?
        public int TargetPlayer { get; }

        // which of the target players pieces is it?
        public int TargetPiece { get; }

        // ctor
        public CollisionInfo(int targetPlayer, int targetPiece)
        {
            TargetPlayer = targetPlayer;
            TargetPiece = targetPiece;
        }
    }
}
