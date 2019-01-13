using System;

namespace MadEngine
{
    // http://www.playluggage.com/ludo-rules
    // ^så jävla många rule variants alltså...
    public static class Ludo
    {
        public static ISession NewGame(int playerCount = 2, int boardLength = 40)
        {
            if (playerCount < 2 || playerCount > 4)
                throw new ArgumentOutOfRangeException(nameof(playerCount));
            if (boardLength < 16 || boardLength > 100)
                throw new ArgumentOutOfRangeException(nameof(boardLength));
            if (boardLength % 8 != 0)
                throw new ArgumentException("Must be a multiple of 8.", nameof(boardLength));

            return new Session(playerCount, new BoardInfo(boardLength));
        }

        // Loads a gamestate.
        public static ISession LoadGame(LudoSave save)
        {
            // TODO: kontrollera att spar-data är giltig
            return new Session(save);
        }
    }
}
