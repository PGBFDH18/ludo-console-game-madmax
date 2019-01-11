using System;

namespace Engine
{
    public static class Piece
    {
        const int BOARD_SIZE = 40; // Hur många rutor är ett varv runt spelplanen.
        const int SAFE_ZONE_LENGTH = BOARD_SIZE / 8; // Hur lång är slutsträckan in till mål (inklusive mål-rutan).
        const int GOAL_PROGRESS = BOARD_SIZE + SAFE_ZONE_LENGTH; // Hur långt behöver vi gå för att gå i mål.

        // startrutan för spelare X (när man flyttar ut en pjäs ur sin bas)
        public static int StartIndex(int player)
        {
            if (player < 0 || player >= 4)
                throw new ArgumentOutOfRangeException(nameof(player), "Spelarindex måste vara mellan 0-3");
            return player * BOARD_SIZE / 4;
        }

        // sista rutan på banan för spelare X (nästa förflyttning går pjäsen in på målsträckan)
        //public static int ExitIndex(int player)
        //{
        //    if (player < 0 || player >= 4)
        //        throw new ArgumentOutOfRangeException(nameof(player), "Spelarindex måste vara mellan 0-3");
        //    return (BOARD_SIZE - 1 + (player * BOARD_SIZE / 4)) % BOARD_SIZE;
        //}

        // räknar ut vart en pjäs hamnar om vi flyttar den.
        public static int Move(int currentProgress, int moveDistance)
        {
            if (currentProgress < 0 || currentProgress >= GOAL_PROGRESS)
                throw new ArgumentOutOfRangeException(nameof(currentProgress)
                    , "Current måste vara mellan 0 och (GOAL_PROGRESS - 1).");
            if (moveDistance < 1 || moveDistance > 6)
                throw new ArgumentOutOfRangeException(nameof(moveDistance)
                    , "MoveDistance måste vara mellan 1-6.");

            int progress = currentProgress + moveDistance;
            if (progress == GOAL_PROGRESS) // vi har gått i mål!
                return Player.GOAL;
            else if (progress > GOAL_PROGRESS) // vi är inne på målområdet, men gick för långt (studsa tillbaka!)
                return GOAL_PROGRESS * 2 - progress;
            else
                return progress;
        }
    }
}
