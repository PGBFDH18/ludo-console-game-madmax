using System;
using MadEngine;

namespace LudoByMadMax
{
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
            int numOfPlayers = GetPlayerCountFromConsole();
            string[] players = new string[numOfPlayers];
            PopulatePlayerNamesFromConsole(players);

            Console.WriteLine();
            Console.WriteLine("Great! Let's start.");
            var session = Ludo.NewGame(numOfPlayers);
            Console.WriteLine(players[session.CurrentPlayer]
                + " has been randomly choosen as the starting player!");

            RunGameLoop(session, players);

            Console.WriteLine();
            Console.WriteLine("\t(Game has ended)");
        }

        static int GetPlayerCountFromConsole()
        {
            while(true) // infinite retry...
            {
                Console.Write("Choose number of players: ");
                if (!int.TryParse(Console.ReadLine(), out int playerCount))
                    Console.WriteLine("You can only use numbers. Try again!");
                else if (playerCount < 2 || playerCount > 4)
                    Console.WriteLine("Game supports 2-4 players. Try again!");
                else
                    return playerCount;
            }
        }

        // loop through the string array, overwriting its contents with input from console.
        static void PopulatePlayerNamesFromConsole(string[] playerNames)
        {
            for (int i = 0; i < playerNames.Length; i++)
            {
                Console.Write($"Enter the name of Player {i + 1}: ");
                string name = Console.ReadLine();
                playerNames[i] = string.IsNullOrWhiteSpace(name) ? $"Player{i + 1}" : name;
            }
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
        static void RunGameLoop(ISession session, string[] players)
        {
            var bi = session.BoardInfo;
            float goalDist = bi.GoalDistance;
            bool wasLucky = false;
            do
            {
                Console.WriteLine();
                PrintRollDie();
                PrintPieceCounts();
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
                            Console.WriteLine($", knocking out {players[session.CurrentPlayer]}!");
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
                    session.MovePiece(baseIndex);
                    Console.WriteLine($"Piece #{baseIndex + 1} was moved out from your base.");
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
                        session.MovePiece(piece - 1);
                        Console.WriteLine($"Piece #{piece} was moved {dist} steps.");
                        PrintCollResult(piece);
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

            void PrintPieceCounts()
            {
                if (session.InBaseCount == session.PieceCount - session.InGoalCount)
                    Console.WriteLine("All your playable pieces are currently in your base.");
                else
                    Console.WriteLine($"Pieces in base: {session.InBaseCount} - Pieces in goal: {session.InGoalCount}");
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

            void PrintCollResult(int piece)
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
                    Console.WriteLine();
                }
            }
        }
    }
}
