using System;

namespace MadEngine
{
    public interface ISession
    {
        // The current player.
        int CurrentPlayer { get; }
        // The current die roll.
        int CurrentDieRoll { get; }
        
        // Static info about the board (size etc.)
        BoardInfo BoardInfo { get; }
        // Number of players.
        int PlayerCount { get; }
        // Number of pieces per player.
        int PieceCount { get; }
        // Who won? (PlayerIndex 0-3 or -1 if no one has won yet)
        int Winner { get; }

        // Get piece [0-3] for the current player.
        PieceInfo GetPiece(int piece);
        // Move piece [0-3] for the current player.
        void MovePiece(int piece);
        // If no move is possible, call this to pass the turn to the next player (NOT legal if a move IS possible!)
        void PassTurn();

        // Get the board position of any piece.
        int GetPiecePosition(int player, int piece);
        // Get the piece at a board position [1 - BoardInfo.Length] (or NULL if position is empty).
        PlayerPiece? LookAtBoard(int position);
    }
}
