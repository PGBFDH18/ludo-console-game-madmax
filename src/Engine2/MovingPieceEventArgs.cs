using System;

namespace Ludo.GameLogic
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class MovingPieceEventArgs : EventArgs
    {
        public MovingPieceEventArgs(int pieceIndex)
        {
            PieceIndex = pieceIndex;
        }

        public int PieceIndex { get; }
    }
}
