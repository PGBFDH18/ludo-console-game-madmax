using System;
using System.Linq;

namespace MadEngine
{
    // var försiktiga så ni inte blandar ihop distance och position!
    internal class Session : ISession
    {
        // special constants:
        const int SPECIAL_POSITION = -1; // base position or goal position (piece distance differentiates them)
        const int PIECE_COUNT = 4; // pieces per player

        // special rules:
        const bool ALLOW_STACKING = false; // 'true' not implemented
        const bool BASE_EXIT_ON_6_GRANTS_MOVE_6 = false; // 'true' not implemented
        const bool ALLOW_GOAL_BOUNCING = false;
        const bool ALLOW_BASE_EXIT_ON_ROLL_1 = false;
        const bool ALLOW_UNLIMITED_ROLL_6 = true; // 'false' not implemented

        // ctor (new game)
        internal Session(int playerCount, BoardInfo boardInfo)
        {
            pieceDistances = new int[playerCount, PIECE_COUNT];
            currentPieces = new PieceInfo[PIECE_COUNT];
            BoardInfo = boardInfo;
            NextMove();
        }

        // ctor (load game)
        internal Session(LudoSave save)
        {
            pieceDistances = save.pieceDistances;
            currentPieces = new PieceInfo[PIECE_COUNT];
            BoardInfo = new BoardInfo(save.BoardLength);
            CurrentPlayer = save.CurrentPlayer;
            CurrentDieRoll = save.CurrentDieRoll;
            ComputePieceInfo();
        }

        public int CurrentPlayer { get; private set; } = -1;

        public int CurrentDieRoll { get; private set; }

        public BoardInfo BoardInfo { get; }

        public int PlayerCount
            => pieceDistances.Length / PIECE_COUNT;

        public int PieceCount
            => PIECE_COUNT;

        public int Winner { get; private set; } = -1;

        public PieceInfo GetPiece(int piece)
            => currentPieces[piece];

        public void MovePiece(int piece)
        {
            if (currentPieces[piece].CanMove)
            {
                if (currentPieces[piece].IsInBase)
                    MoveBasePiece(piece);
                else
                    MoveBoardPiece(piece);
            }
        }

        public void PassTurn()
        {
            if (currentPieces.All(p => p.CanMove == false))
                NextMove();
        }

        public int GetPiecePosition(int player, int piece)
            => CalculatePosition(player, piece);

        public PlayerPiece? LookAtBoard(int position)
        {
            // first check that the position is on the board and not in a collision-free zone:
            if (position >= 0 && position < BoardInfo.Length)
                for (int player = 0; player < PlayerCount; ++player) // loop over players...
                    for (int piece = 0; piece < PIECE_COUNT; ++piece) // ...and their pieces...
                        if (CalculatePosition(player, piece) == position)
                            return new PlayerPiece(player, piece);
            return null;
        }

        public LudoSave GetSave()
            => new LudoSave(CurrentPlayer, CurrentDieRoll, BoardInfo.Length, pieceDistances);

        // public uppåt...
        // private neråt...

        private void NextMove()
        {
            if (CurrentDieRoll != 6)
                CurrentPlayer = (CurrentPlayer + 1) % PlayerCount;
            CurrentDieRoll = random.Next(1, 7);
            ComputePieceInfo();
        }

        private void MoveBasePiece(int piece)
        {
            pieceDistances[CurrentPlayer, piece] = 1;
            CheckMoveCollision(piece);
            if (BASE_EXIT_ON_6_GRANTS_MOVE_6) // om man går ut på 6 OCH flyttar 6 steg kan man få dubbla collisions...
            {
                throw new NotImplementedException("This rule variant is not supported.");
                //TODO: se till att alla PieceInfo.CanMove = false, utom för pjäsen vi precis flyttade + kolla om det blir kollision.
            }
            else
            {
                NextMove();
            }
        }

        private void MoveBoardPiece(int piece)
        {
            int distance = pieceDistances[CurrentPlayer, piece] + CurrentDieRoll;
            if (distance == BoardInfo.GoalDistance) // Piece has reached the goal! (piece leaves play!)
            {
                pieceDistances[CurrentPlayer, piece] = distance;
                if (CheckVictoryCondition())
                    return; // <-- Game finished !!!
            }
            else if (distance > BoardInfo.GoalDistance) // Piece moved too far - it bounces back. (ALLOW_GOAL_BOUNCING)
            {
                pieceDistances[CurrentPlayer, piece] = BoardInfo.GoalDistance * 2 - distance;
            }
            CheckMoveCollision(piece);
            NextMove();
        }

        private void CheckMoveCollision(int piece)
        {
            if (currentPieces[piece].MovedCollision is PlayerPiece ci)
            {
                if (ci.Player != CurrentPlayer)
                    KnockOut(ci);
            }
        }

        private void KnockOut(PlayerPiece ci)
        {
            pieceDistances[ci.Player, ci.Piece] = 0;
        }

        private int CalculatePosition(int player, int piece)
        {
            int distance = pieceDistances[player, piece];
            if (distance == 0 || distance == BoardInfo.GoalDistance)
            {
                // piece is in base or in goal.
                return SPECIAL_POSITION;
            }
            if (BoardInfo.IsInEndZone(distance))
            {
                // we are in the collision-free end-zone, return distance as the position.
                //return distance;
                return distance + player * BoardInfo.EndZoneLength;
            }
            else
            {
                // we are out on the competative track where collisions are possible!
                return (BoardInfo.StartPosition(player) + distance - 1) % BoardInfo.Length;
            }
        }

        private bool CheckVictoryCondition()
        {
            int goal = BoardInfo.GoalDistance;
            if (currentPieces.All(p => p.CurrentDistance == goal))
            {
                Winner = CurrentPlayer;
                // just update all PieceInfo so CanMove is false and IsInGoal is true:
                for (int i = 0; i < currentPieces.Length; ++i)
                    currentPieces[i] = new PieceInfo(goal, SPECIAL_POSITION);
                // Game has ended! (no further state changes should be allowed!)
                return true;
            }
            return false;
        }

        // here we do the heavy lifting! (checking the rules and updating PieceInfo!)
        private void ComputePieceInfo()
        {
            // cache'ar resultat här så vi slipper räkna ut base-exit reglerna flera ggr:
            PieceInfo? baseExitInfo = null;

            for (int i = 0; i < PIECE_COUNT; ++i)
                ComputePieceInfo(i);

            // << här tar ComputePieceInfo() metoden slut, koden under är "bara" lokala hjälpmetoder >>

            void ComputePieceInfo(int piece)
            {
                if (IsPieceInBase(piece)) // (distance == 0)
                {
                    if (baseExitInfo == null) // räkna ut och cache'a värdet om det inte finns...
                        ComputeBaseExitInfo();
                    currentPieces[piece] = baseExitInfo.Value; // <-- använd cache'ade värdet.
                }
                else if (ALLOW_STACKING)
                {
                    throw new NotImplementedException("Stacking / blocking double-pieces are not currently supported.");
                    // TODO: ...
                }
                else
                {
                    int oldDistance = pieceDistances[CurrentPlayer, piece];
                    int newDistance = oldDistance + CurrentDieRoll;
                    int oldPosition = CalculatePosition(CurrentPlayer, piece);
                    if (newDistance == BoardInfo.GoalDistance)
                    {
                        currentPieces[piece] = new PieceInfo(oldDistance, oldPosition, newDistance); // goal!
                        //^ ok to use newDistance instead of newPosition because in the end-zone
                        //  distance and position are the same.
                        return; // <--
                    }
                    if (newDistance > BoardInfo.GoalDistance)
                    {
                        if (ALLOW_GOAL_BOUNCING)
                        {
                            newDistance = BoardInfo.GoalDistance * 2 - newDistance;
                        }
                        else
                        {
                            currentPieces[piece] = new PieceInfo(oldDistance, oldPosition); // cant move.
                            return; // <--
                        }
                    }
                    int newPosition = CalculatePosition(CurrentPlayer, piece);
                    if (LookAtBoard(newPosition) is PlayerPiece ci)
                    {
                        //^ the new position collides with something...
                        if (ci.Player == CurrentPlayer)
                        {
                            //^ new position collides with one of our own pieces
                            if (ci.Piece == piece)
                            {
                                //^ False alarm! Our piece is "colliding" with itself (ALLOW_GOAL_BOUNCING)
                                currentPieces[piece] = new PieceInfo(oldDistance, oldPosition); // cant move...
                                // ...well technically we can "move" but we end up on the same square as we started.
                                // We make this an illegal move since allowing it would just be annoying and useless.
                            }
                            else if (ALLOW_STACKING)
                            {
                                throw new NotImplementedException("Stacking / blocking double-pieces are not currently supported.");
                                // TODO: ...
                            }
                            else
                            {
                                //^ another one of our pieces is in the way
                                currentPieces[piece] = new PieceInfo(oldDistance, oldPosition); // cant move.
                            }
                        }
                        else
                        {
                            //^ new position collides with another players piece
                            if (ALLOW_STACKING)
                            {
                                throw new NotImplementedException("Stacking / blocking double-pieces are not currently supported.");
                                // TODO: ... check if it is a double piece
                            }
                            else
                            {
                                //^ we can kill it!
                                currentPieces[piece] = new PieceInfo(oldDistance, oldPosition, newPosition, ci);
                            }
                        }
                    }
                    else
                    {
                        //^ new position is empty / no collision
                        currentPieces[piece] = new PieceInfo(oldDistance, oldPosition, newPosition);
                    }
                }
            }

            void ComputeBaseExitInfo()
            {
                bool isBaseRoll = CurrentDieRoll == 6 || (CurrentDieRoll == 1 && ALLOW_BASE_EXIT_ON_ROLL_1);
                if (isBaseRoll)
                {
                    int startPosition = BoardInfo.StartPosition(CurrentPlayer);
                    if (LookAtBoard(startPosition) is PlayerPiece ci)
                    {
                        //^ another piece is occupying our startPosition...
                        if (ci.Player == CurrentPlayer)
                        {
                            //^ we already have a piece on our startingPosition...
                            if (ALLOW_STACKING)
                            {
                                throw new NotImplementedException("Stacking / blocking double-pieces are not currently supported.");
                                // TODO: ...
                            }
                            else
                            {
                                baseExitInfo = new PieceInfo(0, SPECIAL_POSITION); // we can not move out of base!
                            }
                        }
                        else
                        {
                            //^ another players piece is on our startPosition...
                            if (ALLOW_STACKING)
                            {
                                throw new NotImplementedException("Stacking / blocking double-pieces are not currently supported.");
                                // TODO: check if the collison target is stacked / a double-piece
                            }
                            else
                            {
                                baseExitInfo = new PieceInfo(0, SPECIAL_POSITION, startPosition, ci);
                            }
                        }
                    }
                    else
                    {
                        //^ startPosition is empty / no collision...
                        baseExitInfo = new PieceInfo(0, SPECIAL_POSITION, startPosition);
                    }
                }
                else // (isBaseRoll == false)
                {
                    baseExitInfo = new PieceInfo(0, SPECIAL_POSITION); // we can not move out of base!
                }
            }
        }

        private bool IsPieceInBase(int piece)
            => pieceDistances[CurrentPlayer, piece] == 0;

        // how far each piece has moved. [player, piece]
        private readonly int[,] pieceDistances;
        private readonly PieceInfo[] currentPieces;
        private readonly Random random = new Random();
    }
}
