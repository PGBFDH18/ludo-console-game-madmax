using System;
using System.Collections.Generic;
using System.Text;

namespace LudoByMadMax
{
    interface IBoardStateProvider
    {

    }

    class BoardViewer
    {
        private readonly IBoardStateProvider provider;

        public BoardViewer(IBoardStateProvider boardStateProvider)
        {
            provider = boardStateProvider;
        }

        private readonly ConsoleColor[] playerColors = new ConsoleColor[]
        {
            ConsoleColor.Red,
            ConsoleColor.Green,
            ConsoleColor.Blue,
            ConsoleColor.Yellow
        };

        public void SetPlayerColor(int player, ConsoleColor color)
            => playerColors[player] = color;

        //private 

        void DrawBoard() { }


    }
}
