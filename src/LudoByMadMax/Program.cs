﻿using System;
using Engine;

namespace LudoByMadMax
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Ludo - by MadMax");
            Console.WriteLine("---------------------------");
            Console.Write("Number of players: ");
            int players = int.Parse(Console.ReadLine());

            for (int i = 1; i <= players; i++)
            {
                Console.Write("Enter the name of Player " + i + ": ");
                string name = Console.ReadLine();
                // create new Player-object. Player.Name = name
            }       


        }
    }
}
