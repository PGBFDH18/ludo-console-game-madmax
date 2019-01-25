using System;

namespace MadEngine
{
    public interface ISession
    {
        // The current player.
        int CurrentPlayer { get; }
        // The current die roll.
        int CurrentDieRoll { get; }

        // The number of pieces the current player has in their base.
        int InBaseCount { get; }
        // The number of pieces the current player has reach the goal with.
        int InGoalCount { get; }
        // True if the current player has at least one piece that can move. (GetPiece(x).CanMove == TRUE for some x)
        bool CanMove { get; }
        // Typically only true if no piece can move. (Unless house-rules are applied that allow passing anyway.)
        bool CanPass { get; }
        // True if the current player has another move after their current move.
        bool IsLucky { get; }

        // Static info about the board (size etc.)
        BoardInfo BoardInfo { get; }
        // Number of players.
        int PlayerCount { get; }
        // Number of pieces per player.
        int PieceCount { get; }
        // Who won? (PlayerIndex 0-3 or -1 if no one has won yet)
        int Winner { get; }

        // Get info about piece [0-3] for the current player.
        PieceInfo GetPiece(int piece);
        // Move piece [0-3] for the current player (and proceed to the next turn / roll die).
        void MovePiece(int piece);
        // Moves a piece out from the current players base (and proceed to the next turn / roll die).
        void MoveBasePiece();
        // Call this to pass the turn to the next player, or if lucky, simply re-roll the die.
        void PassTurn();

        // Get the board position of any piece.
        int GetPiecePosition(int player, int piece);
        // Get the piece at a board position [1 - BoardInfo.Length] (or NULL if position is empty).
        PlayerPiece? LookAtBoard(int position);

        // Returns the current gamestate.
        LudoSave GetSave();
    }
}
