using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    public class Session
    {
        private Random random = new Random();
        //private List<Player> players;

        public Session(int playerCount = 2)
        {
            if (playerCount < 2 || playerCount > 4)
                throw new ArgumentException("Number of players must be between 2-4.");
            PlayerCount = playerCount;
            NextPlayer();
        }

        private void NextPlayer()
        {
            CurrentPlayer = (CurrentPlayer + 1) % PlayerCount;
            CurrentDieRoll = RollDie();
        }

        private int RollDie()
        {
            return random.Next(1, 7);
        }

        public int PlayerCount { get; }

        public int CurrentPlayer { get; private set; } = -1;

        public int CurrentDieRoll { get; private set; }
        
        public PlayerAction CurrentOptions
        {
            get
            {
                throw new NotImplementedException(); //TODO
            }
        }

        public Result ChooseOption(PlayerAction choice)
        {
            throw new NotImplementedException();
            //TODO
        }
    }

    public enum PlayerAction
    {
        DoNothing = 0,
        MovePiece1,
        MovePiece2,
        MovePiece3,
        MovePiece4,
        //TODO
    }

    public class Result
    {
     // ????
    }
}
