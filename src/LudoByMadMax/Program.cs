using System;
using MadEngine;
using System.Collections.Generic;

namespace LudoByMadMax
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Ludo - by MadMax");
            Console.WriteLine("---------------------------");
            int numOfPlayers = 0;
            while (true)
            {
                Console.Write("Number of players: ");
                try
                {
                    numOfPlayers = int.Parse(Console.ReadLine());
                    if (numOfPlayers < 2 || numOfPlayers > 4)
                    {
                        Console.WriteLine("Choose between 2-4 players!");
                    }
                    else
                        break;
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine("You can only use numbers. Try again!");
                }
            }

            string[] players = new string[numOfPlayers];

            for (int i = 0; i < numOfPlayers; i++)
            {
                Console.Write("Enter the name of Player " + (i + 1) + ": ");
                string name = Console.ReadLine();
                players[i] = name;
            }

            Console.WriteLine("Great! Let's start.");
            Console.WriteLine();

            var session = Ludo.NewGame(numOfPlayers);
            bool gameActive = true;
            Console.WriteLine(players[0] + " begins!");
            Console.WriteLine();

            while (gameActive)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    Console.WriteLine(players[i] + "! Hit a key to roll the die.");
                    Console.ReadKey();
                    Console.WriteLine("You got " + session.CurrentDieRoll + "!");
                    int piecesInBase = CountPiecesInBase(session);
                    Console.WriteLine("Pieces in nest: " + piecesInBase);
                    Console.Write("Pieces out (piece/position): ");
                    int piecesOut = session.PieceCount - piecesInBase;
                    if (piecesOut > 0)
                    {
                        for (int j = 0; j < session.PieceCount; j++)
                        {
                            var piece = session.GetPiece(j);
                            if (!piece.IsInBase && !piece.IsInGoal)
                                Console.Write(j + 1 + "({0}), ", piece.CurrentPosition);
                        }
                    }
                    else
                    {
                        Console.WriteLine(piecesOut);
                    }
                }
            }
        }

        static int CountPiecesInBase(ISession session)
        {
            int count = 0;
            for (int i = 0; i < session.PieceCount; ++i)
                if (session.GetPiece(i).IsInBase)
                    ++count;
            return count;
        }
    }
}
