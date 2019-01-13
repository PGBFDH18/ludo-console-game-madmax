using System;

namespace MadEngine
{
    public struct LudoSave
    {
        public int CurrentPlayer { get; }
        public int CurrentDieRoll { get; }
        public int BoardLength { get; }

        public int PlayerCount 
            => pieceDistances.GetLength(0);

        public int GetPieceDistance(int player, int piece)
            => pieceDistances[player, piece];

        internal readonly int[,] pieceDistances;

        // ctor
        public LudoSave(int player, int dieRoll, int boardLength, int[,] playerPieceDistances)
        {
            CurrentPlayer = player;
            CurrentDieRoll = dieRoll;
            BoardLength = boardLength;
            pieceDistances = playerPieceDistances;
        }

        public LudoSave(string savedState)
        {
            throw new NotImplementedException("Loading state from string not implemented yet.");
        }

        public override string ToString()
        {
            return base.ToString(); // TODO: change this to return JSON or similar.
        }
    }
}
