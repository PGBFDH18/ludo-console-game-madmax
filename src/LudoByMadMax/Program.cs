using System;
using Ludo.Bots;
using Ludo.GameLogic;

namespace LudoByMadMax
{
    // This has grown into a huge mess in need of refactoring...
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Ludo - by MadMax");
            Console.WriteLine("---------------------------");
            ConsoleKey key;
            do
            {
                NewGame();
                Console.WriteLine("Press 'R' to play again, or the any key to quit.");
                key = Console.ReadKey(true).Key;
            }
            while (key == ConsoleKey.R);
        }

        static void NewGame()
        {
            GetPlayerCountsFromConsole(out int humanCount, out int botCount);
            int playerCount = humanCount + botCount;
            string[] players = new string[playerCount];
            GetHumanPlayerNamesFromConsole(players, humanCount);

            Console.WriteLine();
            Console.WriteLine("Creating game session...");
            var rules = new Rules(allowGoalBouncing: false, allowBaseExitOnRoll1: true); // <--- TODO
            var session = SessionFactory.New(playerCount, rules);

            var bots = new LudoBot[botCount];
            if (botCount != 0)
            {
                Console.WriteLine("Creating bots...");
                CreateAndNameBots(session, players, bots);
            }

            Console.WriteLine(players[session.CurrentPlayer]
                + " has been randomly choosen as the starting player!");
            Console.WriteLine();

            var observer = botCount == 0 ? null : new BotObserver(session, players, humanCount, true);
            foreach (var bot in bots)
                bot.TryMakeMove();

            RunGameLoop(session, players, humanCount);

            Console.WriteLine();
            Console.WriteLine("\t(Game has ended)");

            observer?.Dispose();
            foreach (var bot in bots)
                bot.Dispose();
        }

        static void GetPlayerCountsFromConsole(out int humans, out int bots)
        {
            while(true) // infinite retry...
            {
                Console.Write("Choose number of human players: ");
                if (!int.TryParse(Console.ReadLine(), out humans) || humans < 1)
                    Console.WriteLine("You can only use positive numbers. Try again!");
                else if (humans > 4)
                    Console.WriteLine("Game supports max 4 players. Try again!");
                else
                    break;
            }
            if (humans == 4)
            {
                bots = 0;
                return;
            }
            bool invalid;
            do
            {
                invalid = false;
                Console.Write("Choose number of bots: ");
                var line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    bots = 0;
                else if (invalid = (!int.TryParse(line, out bots) || bots < 0))
                    Console.WriteLine("You can only use non-negative numbers. Try again!");
                else if (invalid = (bots > 4 - humans))
                    Console.WriteLine($"Game supports max 4 players, so there is room for max {4 - humans} bots. Try again!");
                if (invalid |= (humans == 1 && bots == 0))
                    Console.WriteLine("Game requires at least 2 players. Try again!");
            }
            while (invalid);
        }

        // loop through the string array, overwriting its contents with input from console.
        static void GetHumanPlayerNamesFromConsole(string[] playerNames, int humans)
        {
            for (int i = 0; i < humans; i++)
            {
                Console.Write($"Enter the name of Player {i + 1}: ");
                string name = Console.ReadLine();
                playerNames[i] = string.IsNullOrWhiteSpace(name) ? $"Player{i + 1}" : name;
            }
        }

        static void CreateAndNameBots(ISession session, string[] players, LudoBot[] bots)
        {
            int humanCount = players.Length - bots.Length;
            for (int i = 0; i < bots.Length; ++i)
            {
                var bot = CreateRandomBot();
                bots[i] = bot;
                int playerIndex = humanCount + i;
                players[playerIndex] = bot.StaticName + $" ({i})";
                bot.Register(session, playerIndex, false);
            }
        }

        static LudoBot CreateRandomBot()
        {
            // if more bot implementations are made, add them here and pick one at random...
            return new RngesusBot();
            // right now we only have one bot implementation, but the method name fits anyway :P
        }

        static bool QuitConfirm(ConsoleKey key)
        {
            return key == ConsoleKey.Q && QuitConfirm();
        }

        static bool QuitConfirm()
        {
            Console.WriteLine("Press Q again to confirm or the any key to return to the game.");
            return Console.ReadKey(true).Key == ConsoleKey.Q;
        }

        static void ShowBoard(ISession session)
        {
            // TODO ------------------- TODO
            Console.WriteLine("My apologies...");
            Console.WriteLine("I fear the lazy programmer gnomes have yet to finish your requested feature.");
            // TODO ------------------- TODO
        }

        // runs until a winner has been crowned.
        static void RunGameLoop(ISession session, string[] players, int humans)
        {
            var bi = session.BoardInfo;
            float goalDist = bi.GoalDistance;
            bool wasLucky = false;
            do
            {
                //if (!IsHuman())
                //    continue;
                Console.WriteLine();
                PrintRollDie();
                PrintPieceCounts(session);
                PrintNonBasePieces();
                if (!session.CanMove)
                {
                    PrintNoMove(out bool quit);
                    if (quit)
                        return; // <---
                    session.PassTurn();
                }
                else
                {
                    // this returns the index of the first piece in base that can move, or -1.
                    int baseIndex = PrintChoices();
                    bool readAgain;
                    do
                    {
                        if (!HandleChoice(ReadChoice(), baseIndex, out readAgain, out bool quit))
                            Console.WriteLine("Invalid choice, try again!");
                        else if (quit)
                            return; // <---
                    }
                    while (readAgain);
                }
            }
            while (session.Winner == -1);

            Console.WriteLine();
            Console.WriteLine($"Congratulations {players[session.Winner]}, you have won the game!");

            // <--- The end of RunGameLoop method --->
            // <--- ...local helper methods below --->

            int PrintChoices()
            {
                Console.WriteLine("These are your options...");

                int baseIndex = -1;
                for (int i = 0; i < session.PieceCount; ++i)
                {
                    var p = session.GetPiece(i);
                    if (!p.CanMove)
                        continue;
                    if (p.IsInBase)
                    {
                        // även om flera pjäser kan flyttas ut räcker det att nämna den första
                        if (baseIndex == -1)
                        {
                            Console.WriteLine("\tB - Move a piece out from your base.");
                            baseIndex = i;
                        }
                    }
                    else
                    {
                        Console.Write($"\t{i+1} - piece move");
                        if (p.Collision is PlayerPiece mc)
                            Console.WriteLine($", knocking out {players[mc.Player]}!");
                        else
                            Console.WriteLine(".");
                    }
                }
                if (session.CanPass)
                    Console.WriteLine("\t0 - Pass your turn.");
                Console.WriteLine("\tS - Show the board.");
                Console.WriteLine("\tQ - Quit game.");

                return baseIndex;
            }

            ConsoleKeyInfo ReadChoice()
            {
                Console.Write(" > ");
                var key = Console.ReadKey();
                Console.WriteLine();
                return key;
            }

            bool HandleChoice(ConsoleKeyInfo key, int baseIndex, out bool readAgain, out bool quit)
            {
                readAgain = quit = false;
                if (key.Key == ConsoleKey.Q)
                {
                    quit = QuitConfirm();
                    readAgain = !quit;
                    return true;
                }
                if (key.Key == ConsoleKey.B && baseIndex != -1)
                {
                    Console.WriteLine($"Piece #{baseIndex + 1} is moved out from your base.");
                    session.MovePiece(baseIndex);
                    return true;
                }
                if (key.Key == ConsoleKey.S)
                {
                    ShowBoard(session);
                    readAgain = true;
                    return true;
                }
                if (session.CanPass && key.KeyChar == '0')
                {
                    session.PassTurn();
                    return true;
                }
                if (int.TryParse(new string(key.KeyChar, 1), out int piece)
                    && piece > 0 && piece <= session.PieceCount)
                {
                    var p = session.GetPiece(piece - 1);
                    if (p.CanMove && !p.IsInBase)
                    {
                        int dist = session.CurrentDieRoll;
                        Console.WriteLine($"Piece #{piece} is moved {dist} steps.");
                        PrintCollResult(session, players, piece);
                        session.MovePiece(piece - 1);
                        return true;
                    }
                }
                readAgain = true;
                return false;
            }

            void PrintRollDie()
            {
                Console.WriteLine(players[session.CurrentPlayer] + " - Hit a key to roll the die" +
                    (wasLucky ? " again." : "."));
                wasLucky = session.IsLucky;
                Console.ReadKey(true);
                Console.WriteLine($"You rolled a {session.CurrentDieRoll}" +
                    (wasLucky ? "!" : "."));
            }

            void PrintNonBasePieces()
            {
                for (int i = 0; i < session.PieceCount; ++i)
                {
                    var p = session.GetPiece(i);
                    if (p.IsInBase || p.IsInGoal)
                        continue;

                    PrintPos(i, in p);
                    if (p.CanMove)
                    {
                        PrintMove(in p);
                        PrintCollPreview(in p);
                    }
                    else
                    {
                        PrintBlock(in p);
                    }
                }
            }

            void PrintPos(int i, in PieceInfo p)
            {
                Console.Write($"Piece #{i+1} is at position @{p.CurrentPosition} on the board");
                if (bi.IsInEndZone(p.CurrentDistance))
                    Console.Write(" (ENDZONE)");
                Console.WriteLine($" ({p.CurrentDistance * 100 / goalDist:F1}%).");
            }

            void PrintMove(in PieceInfo p)
            {
                Console.Write($"  - It can be moved to position @{p.MovedPosition}");
                int movedDistance = p.CurrentDistance + session.CurrentDieRoll;
                if (bi.IsInEndZone(movedDistance))
                    Console.Write(" (ENDZONE)");
                Console.WriteLine($" ({movedDistance * 100 / goalDist:F1}%).");
            }

            void PrintCollPreview(in PieceInfo p)
            {
                if (p.Collision is PlayerPiece mc)
                    Console.WriteLine($"\tThere it collides with {players[mc.Player]} #{mc.Piece + 1} piece.");
            }

            void PrintBlock(in PieceInfo p)
            {
                if (p.Collision is PlayerPiece mc)
                    Console.WriteLine("  - It cannot move because it is blocked by" +
                        (mc.Player == session.CurrentPlayer
                        ? $" your own #{mc.Piece + 1} piece."
                        : $" {players[mc.Player]} #{mc.Piece + 1} piece."));
            }

            void PrintNoMove(out bool quit)
            {
                Console.WriteLine("Unfortunately you cannot make any move. (>_<)");
                if (session.IsLucky)
                {
                    Console.WriteLine("But got a lucky roll, so you can roll again!");
                    quit = false;
                }
                else
                {
                    Console.WriteLine("Hit a key to pass. (Or Q to quit)");
                    if (quit = QuitConfirm(Console.ReadKey(true).Key))
                        return;
                    //Console.WriteLine();
                }
            }
        }

        internal static void PrintPieceCounts(ISession session)
        {
            if (session.InBaseCount == session.PieceCount - session.InGoalCount)
                Console.WriteLine("All your playable pieces are currently in your base.");
            else
                Console.WriteLine($"Pieces in base: {session.InBaseCount} - Pieces in goal: {session.InGoalCount}");
        }

        internal static void PrintCollResult(ISession session, string[] players, int piece)
        {
            if (session.GetPiece(piece).Collision is PlayerPiece mc)
            {
                if (mc.Player == session.CurrentPlayer) // stacking!
                {
                    throw new NotImplementedException("stacking not supported by UI");
                }
                else
                {
                    Console.WriteLine($"{players[mc.Player]}'s #{mc.Piece + 1} piece was knocked out / moved back to base!");
                }
            }
        }
    }
}
