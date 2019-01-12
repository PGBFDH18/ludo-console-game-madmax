using System;

namespace Engine
{
    public class Player
    {
        public const int BASE = -1; // Pjäsen är i basen.
        public const int GOAL = -2; // Pjäsen har gått i mål.

        private readonly int[] pieceProgress = new int[] { BASE, BASE, BASE, BASE };

        // hur många steg har pjäs X gått på brädet.
        public int GetProgress(int piece)
            => pieceProgress[piece];

        // var på brädet är pjäs X.
        public int GetBoardPosition(int piece)
            => pieceProgress[piece] + Piece.StartIndex(playerIndex);

        // hur många pjäser har spelaren i basen.
        public int PiecesInBaseCount { get; private set; }

        // är pjäs X i basen.
        public bool IsPieceInBase(int piece)
            => pieceProgress[piece] == BASE;

        // har pjäs X gått i mål (inte längre spelbar).
        public bool IsPieceInGoal(int piece)
            => pieceProgress[piece] == GOAL;

        public Player(int playerIndex)
        {
            this.playerIndex = playerIndex;

            int pieceInBaseCount = 0;
            foreach (var p in pieceProgress)
            {
                if (p == BASE)
                    pieceInBaseCount++;
            }
            //GetPiecesInBase = pieceInBaseCount;
        }

        private readonly int playerIndex;
        
        //public bool TryMovePiece(int piece, int distance)
        //{
            
        //}
        
        public void MovePiece(int piece, int distance)
        {
            pieceProgress[piece] = Piece.Move(pieceProgress[piece], distance);
        }

        // argument how to enter last stage "special winning area"
        public void  EndGameCanEnter(int dice = 6, bool HomePosition = true)
        {   

        }
    }

}
