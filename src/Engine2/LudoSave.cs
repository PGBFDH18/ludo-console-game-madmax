using System;

namespace Ludo.GameLogic
{
    public struct LudoSave
    {
        public int TurnCounter { get; }
        public int CurrentPlayer { get; }
        public int CurrentDieRoll { get; }
        public int BoardLength { get; }
        public Rules Rules { get; }

        public int PlayerCount 
            => pieceDistances.GetLength(0);

        public int GetPieceDistance(int player, int piece)
            => pieceDistances[player, piece];

        internal readonly int[,] pieceDistances;

        // ctor
        public LudoSave(int turn, int player, int dieRoll, int boardLength, Rules rules, int[,] playerPieceDistances)
        {
            TurnCounter = turn;
            CurrentPlayer = player;
            CurrentDieRoll = dieRoll;
            BoardLength = boardLength;
            pieceDistances = playerPieceDistances;
            Rules = rules;
        }
    }
}
