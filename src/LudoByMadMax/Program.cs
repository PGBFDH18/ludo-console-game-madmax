using System;
using Engine;
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
                Console.Write("Enter the name of Player " + (i++) + ": ");
                string name = Console.ReadLine();
                players[i] = name;
            }

            Console.WriteLine("Great! Let's start.");
            Console.WriteLine();

            var session = new Session(numOfPlayers);
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
                    Console.WriteLine("Pieces in base: " /**/);
                    Console.WriteLine("Pieces out (piece/position): "/**/);
                }
            }
        }
    }
}
