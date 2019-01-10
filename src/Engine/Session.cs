using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    public class Session
    {   
        private readonly Random random = new Random();
        private readonly Player[] players;

        public Session(int playerCount = 2)
        {
            if (playerCount < 2 || playerCount > 4)
            {
                throw new ArgumentException("Number of players must be between 2-4.");
            }
            PlayerCount = playerCount;
            players = CreatePlayerArray(playerCount);
            NextPlayer();
        }

        private static Player[] CreatePlayerArray(int playerCount)
        {
            var players = new Player[playerCount];
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = new Player();
            }
            return players;
        }

        private void NextPlayer()
        {
            CurrentPlayer = (CurrentPlayer + 1) % PlayerCount;
            CurrentDieRoll = RollDie();
            CurrentPiecesInBase = players[CurrentPlayer].GetPiecesInBase;
        }

        private int RollDie()
        {
            return random.Next(1, 7);
        }

        public int PlayerCount { get; }

        public int CurrentPlayer { get; private set; } = -1;

        public int CurrentDieRoll { get; private set; }

        public int CurrentPiecesInBase { get; private set; }

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
