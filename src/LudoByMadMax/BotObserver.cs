using System;
using Ludo.GameLogic;

namespace LudoByMadMax
{
    class BotObserver : IDisposable
    {
        public BotObserver(ISession session, string[] players, int humans, bool invokeTurnBegun)
        {
            this.session = session;
            this.players = players;
            this.humans = humans;
            Subscribe();
            if (invokeTurnBegun && !CurrentPlayerIsHuman)
                PrintTurnBegun(CurrentName, session.CurrentDieRoll);
        }

        public bool CurrentPlayerIsHuman
            => session.CurrentPlayer < humans;

        public string CurrentName
            => players[session.CurrentPlayer];

        public static void PrintTurnBegun(string name, int dieRoll)
            => Console.WriteLine(name + $" takes its turn. It rolled a {dieRoll}.");

        public static void PrintPassingTurn(string name, bool isLucky)
            => Console.WriteLine(name + (isLucky ? " takes its next turn." : " passed the turn."));

        private void Subscribe()
        {
            session.TurnBegun += Session_TurnBegun;
            session.PassingTurn += Session_PassingTurn;
            session.MovingPiece += Session_MovingPiece;
        }

        private void Unsubscribe()
        {
            session.TurnBegun -= Session_TurnBegun;
            session.PassingTurn -= Session_PassingTurn;
            session.MovingPiece -= Session_MovingPiece;
        }

        private void Session_MovingPiece(object sender, MovingPieceEventArgs e)
        {
            if (CurrentPlayerIsHuman)
                return;
            var p = session.GetPiece(e.PieceIndex);
            if (p.IsInBase)
            {
                Console.WriteLine(CurrentName + " moved a piece out from their base.");
                //Console.WriteLine($"  - It now has {session.PieceCount - session.InBaseCount - session.InGoalCount + 1} piece(s) on the board.");
                Console.WriteLine($"Pieces in base: {session.InBaseCount - 1} - Pieces in goal: {session.InGoalCount}");
            }
            else
            {
                Console.WriteLine(CurrentName + $" moved its {ordinal[e.PieceIndex]} piece {session.CurrentDieRoll} steps. (From {p.CurrentPosition} to {p.MovedPosition})");
            }
            Program.PrintCollResult(session, players, e.PieceIndex); // <-------------------- FIXME / ugly shit.
        }

        private void Session_PassingTurn(object sender, EventArgs e)
        {
            if (CurrentPlayerIsHuman)
                return;
            PrintPassingTurn(CurrentName, session.IsLucky);
        }

        private void Session_TurnBegun(object sender, EventArgs e)
        {
            if (CurrentPlayerIsHuman)
                return;
            Console.WriteLine();
            PrintTurnBegun(CurrentName, session.CurrentDieRoll);
        }

        readonly string[] ordinal = new[] { "first", "second", "third", "fourth" };
        readonly ISession session;
        readonly string[] players;
        readonly int humans;

        protected virtual void Dispose(bool disposing)
        {
            Unsubscribe();
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}