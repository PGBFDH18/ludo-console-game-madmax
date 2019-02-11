using System;
using Ludo.GameLogic;

namespace Ludo.Bots
{
    public sealed class RngesusBot : LudoBot
    {
        public override string StaticName => "Rngesus Bot";

        protected override void MakeMove(ISession session)
        {
            int count = CountAndPopulateValidIndexes(session, validIndexes);
            if (count != 0)
                session.MovePiece(validIndexes[rng.Next(count)]);
            else
                session.PassTurn();
        }

        protected override void OnRegistered()
        {
            var s = Session; // must be assigned to a local and null-checked! (weak-ref)
            if (s != null)
                validIndexes = new int[s.PieceCount];
        }

        // only the indexes below count are populated / valid! 
        private static int CountAndPopulateValidIndexes(ISession session, int[] validIndexes)
        {
            int count = 0;
            for (int i = 0; i < session.PieceCount; ++i)
                if (session.GetPiece(i).CanMove)
                    validIndexes[count++] = i;
            return count;
        }

        private readonly Random rng = new Random();
        private int[] validIndexes;
    }
}
