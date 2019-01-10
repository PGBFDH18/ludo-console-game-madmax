using System;

namespace Engine
{
    public class Player
    {
        const int BASE = -1; // Pjäsen är i basen
        const int GOAL = -2; // Pjäsen har gått i mål

        private readonly int[] piecePositions = new int[] { BASE, BASE, BASE, BASE };

        public int GetPiecePosition(int piece)
            => piecePositions[piece];

        public int GetPiecesInBase { get; private set; }

        public bool IsPieceInBase(int piece)
            => piecePositions[piece] == BASE;

        public Player()
        {
            int pieceInBaseCount = 0;
            foreach (var p in piecePositions)
            {
                if (p == BASE)
                    pieceInBaseCount++;
            }
        }
        
        //public bool TryMovePiece(int piece, int distance)
        //{
            
        //}

        public void MovePiece(int piece, int distance)
        {

        }

        // argument how to enter last stage "special winning area"
        public void  EndGameCanEnter(int dice = 6, bool HomePosition = true)
        {   
 
        }

    }

}
