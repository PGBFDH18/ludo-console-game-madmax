using System;

namespace Engine
{
    public class Player
    {
        private static Random random = new Random();

        public string Name { get; set; }

        private readonly int[] piecePositions = new int[] { -1, -1, -1, -1 };

        public Player(string name)
        {
            Name = name;
        }

        public int GetPiecePosition(int piece)
            => piecePositions[piece];

        public bool IsPieceInNest(int piece)
            => piecePositions[piece] == -1;
        
        //public bool TryMovePiece(int piece, int distance)
        //{
            
        //}

        public static int RollDice()
            => random.Next(1, 7);

        public void MovePiece(int piece, int distance)
        {

        }
        // argument how to enter last stage "special winning area"
        public void  EndGameCanEnter(int dice = 6, bool HomePosistion = true)
        {   
 
        }

    }

}
