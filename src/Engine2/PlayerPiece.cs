using System;

namespace Ludo.GameLogic
{
    public struct PlayerPiece
    {
        // who does the colliding piece belong to?
        public int Player { get; }

        // which of the target players pieces is it?
        public int Piece { get; }

        // ctor
        public PlayerPiece(int player, int piece)
        {
            Player = player;
            Piece = piece;
        }
    }
}
