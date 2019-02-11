using System;

namespace Ludo.GameLogic
{
    public interface ISession
    {
        // A new turn has begun. (Informational: Not accepting input.)
        event EventHandler TurnBegun;
        // The CurrentPlayer is passing their turn. (Turn has not passed yet!) (Informational: Not accepting input.)
        event EventHandler PassingTurn;
        // The CurrentPlayer is moving a piece. (The piece has not moved yet!) (Informational: Not accepting input.)
        event EventHandler<MovingPieceEventArgs> MovingPiece;
        // A piece has been knocked out by another piece (and thus moved back to its base). (Informational: Not accepting input.)
        //event EventHandler<TODO> PieceKnockedOut;
        // A player has won the game! (Informational: Not accepting input.)
        event EventHandler WinnerDeclared;

        // Calling MovePiece and PassTurn is only valid while this is true.
        bool IsAcceptingInput { get; }
        // Started accepting input. (Useful for implementing bots!)
        // IMPORTANT: Remaining subscribers are NOT invoked if an invoked subscriber supplies input.
        event EventHandler AcceptingInput;

        // The current turn number (i.e. a counter for how many times the die has been rolled).
        int TurnCounter { get; }

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
