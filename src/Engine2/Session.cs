using System;
using System.Collections.Generic;
using System.Linq;

namespace Ludo.GameLogic
{
    // var försiktiga så ni inte blandar ihop distance och position!
    internal class Session : ISession
    {
        // special constants:
        const int NON_BOARD_POSITION = -1; // base or goal position (distance tells them apart)
        const int PIECE_COUNT = 4; // pieces per player

        // special rules TODO:
        const bool ALLOW_STACKING = false; // 'true' not implemented
        const bool ALLOW_UNLIMITED_ROLL_6 = true; // 'false' not implemented

        // <ctors>

        // ctor (new game)
        // startingPlayer == -1 means random starting player.
        internal Session(int playerCount, BoardInfo boardInfo, Rules rules, int startingPlayer = -1)
        {
            pieceDistances = new int[playerCount, PIECE_COUNT];
            currentPieces = new PieceInfo[PIECE_COUNT];
            BoardInfo = boardInfo;
            Rules = rules;
            CurrentPlayer = (startingPlayer < 0 ? random.Next() : startingPlayer) % playerCount;
            RollDie();
            ComputePieceInfo();
            InvokeInitialEvents();
        }

        // ctor (load game)
        internal Session(LudoSave save)
        {
            TurnCounter = save.TurnCounter;
            pieceDistances = save.pieceDistances;
            currentPieces = new PieceInfo[PIECE_COUNT];
            BoardInfo = new BoardInfo(save.BoardLength);
            Rules = save.Rules;
            CurrentPlayer = save.CurrentPlayer;
            CurrentDieRoll = save.CurrentDieRoll;
            ComputePieceInfo();
            InvokeInitialEvents();
        }

        // </ctors> <events>

        public event EventHandler TurnBegun;

        public event EventHandler PassingTurn;

        public event EventHandler<MovingPieceEventArgs> MovingPiece;

        public event EventHandler WinnerDeclared;

        public event EventHandler AcceptingInput;

        //</events> <public>

        public bool IsAcceptingInput { get; private set; }
        public int TurnCounter { get; private set; }
        public int CurrentPlayer { get; private set; }
        public int CurrentDieRoll { get; private set; }
        public int InBaseCount { get; private set; }
        public int InGoalCount { get; private set; }
        public int Winner { get; private set; } = -1;
        public BoardInfo BoardInfo { get; }
        public Rules Rules { get; }

        public int PlayerCount
            => pieceDistances.Length / PIECE_COUNT;

        public int PieceCount
            => PIECE_COUNT;

        public PieceInfo GetPiece(int piece)
            => currentPieces[piece];

        public bool CanMove { get; private set; }
        public bool CanPass => !CanMove; // TODO: house-rules

        //public void MoveBasePiece()
        //{
        //    if (!currentPieces.FirstIndex(p => p.IsInBase && p.CanMove, out int i))
        //        throw new LudoRuleException("Rules does not allow the current player to " + "move a piece out of their base.");
        //    MoveBasePiece(i);
        //}

        public void MovePiece(int piece)
        {
            if (!IsAcceptingInput)
                throw new InvalidOperationException("Session is not in an input accepting state!");
            if (!currentPieces[piece].CanMove)
                throw new LudoRuleException("Rules does not allow the current player to " + $"move piece #{piece}.");
            BlockInput();
            if (currentPieces[piece].IsInBase)
                MoveBasePiece(piece);
            else
                MoveBoardPiece(piece);
        }

        public void PassTurn()
        {
            if (!IsAcceptingInput)
                throw new InvalidOperationException("Session is not in an input accepting state!");
            if (!CanPass)
                throw new LudoRuleException("Rules does not allow the current player to " + "pass the turn.");
            BlockInput();
            OnPassingTurn();
            NextTurn();
        }

        public int GetPiecePosition(int player, int piece)
            => CalculatePosition(player, piece);

        public PlayerPiece? LookAtBoard(int position)
        {
            if (position >= 0 && position < BoardInfo.GoalPosition(3)) // quick range check.
                for (int player = 0; player < PlayerCount; ++player) // loop over players...
                    for (int piece = 0; piece < PIECE_COUNT; ++piece) // and their pieces...
                        if (CalculatePosition(player, piece) == position) // ...find match.
                            return new PlayerPiece(player, piece);
            return null;
        }

        public LudoSave GetSave()
            => new LudoSave(TurnCounter, CurrentPlayer, CurrentDieRoll, BoardInfo.Length, Rules, pieceDistances);

        public bool IsLucky
            => CurrentDieRoll == 6; // TODO: implement rule that limits re-rolls to max three moves in a row.

        // </public>  <protected>

        protected virtual void OnTurnBegun(EventArgs e = null)
            => TurnBegun?.Invoke(this, e ?? EventArgs.Empty);

        protected virtual void OnPassingTurn(EventArgs e = null)
            => PassingTurn?.Invoke(this, e ?? EventArgs.Empty);

        protected virtual void OnMovingPiece(MovingPieceEventArgs e)
            => MovingPiece?.Invoke(this, e);

        protected virtual void OnWinnerDeclared(EventArgs e = null)
            => WinnerDeclared?.Invoke(this, e ?? EventArgs.Empty);

        protected virtual void OnAcceptingInput(EventArgs e = null)
        {
            if (e == null)
                e = EventArgs.Empty;
            int tc = TurnCounter;
            var ai = AcceptingInput;
            if(ai != null)
                foreach (EventHandler eh in ai.GetInvocationList())
                {
                    eh(this, e);
                    if (tc != TurnCounter || !IsAcceptingInput)
                        return;
                }
        }

        protected bool IsPieceInBase(int piece)
            => pieceDistances[CurrentPlayer, piece] == 0;

        protected bool IsPieceInGoal(int piece)
            => pieceDistances[CurrentPlayer, piece] == BoardInfo.GoalDistance;

        protected bool IsBaseRoll
            => CurrentDieRoll == 6 || (CurrentDieRoll == 1 && Rules.AllowBaseExitOnRoll1);

        // </protected>  <private>

        private void InvokeInitialEvents()
        {
            OnTurnBegun();
            AcceptInput();
        }

        private void NextTurn()
        {
            if (!IsLucky)
                CurrentPlayer = (CurrentPlayer + 1) % PlayerCount;
            RollDie();
            ComputePieceInfo();
            ++TurnCounter;
            OnTurnBegun();
            AcceptInput();
        }

        private void RollDie()
        {
            CurrentDieRoll = random.Next(1, 7);
        }

        private void AcceptInput()
        {
            IsAcceptingInput = true;
            OnAcceptingInput();
        }

        private void BlockInput()
        {
            IsAcceptingInput = false;
        }

        private void MoveBasePiece(int piece)
        {
            OnMovingPiece(new MovingPieceEventArgs(piece));
            pieceDistances[CurrentPlayer, piece] = 1;
            HandleMoveCollision(piece);
            NextTurn();
        }

        private void MoveBoardPiece(int piece)
        {
            OnMovingPiece(new MovingPieceEventArgs(piece));
            int distance = pieceDistances[CurrentPlayer, piece] + CurrentDieRoll;
            if (distance > BoardInfo.GoalDistance) // Piece moved too far - it bounces back. (Rules.AllowGoalBouncing)
            {
                pieceDistances[CurrentPlayer, piece] = Bounce(distance);
            }
            else
            {
                pieceDistances[CurrentPlayer, piece] = distance;
                if (CheckVictoryCondition())
                    return; // <-- Game finished !!!
            }
            HandleMoveCollision(piece);
            NextTurn();
        }

        private int Bounce(int distance)
            => BoardInfo.GoalDistance * 2 - distance;

        private void HandleMoveCollision(int piece)
        {
            if (currentPieces[piece].Collision is PlayerPiece ci)
            {
                if (ci.Player != CurrentPlayer)
                    KnockOut(ci);
            }
        }

        private void KnockOut(PlayerPiece pp)
        {
            pieceDistances[pp.Player, pp.Piece] = 0;
        }

        private int CalculatePosition(int player, int piece)
        {
            int distance = pieceDistances[player, piece];
            if (distance == 0 || distance == BoardInfo.GoalDistance)
            {
                // piece is in base or in goal.
                return NON_BOARD_POSITION;
            }
            if (BoardInfo.IsInEndZone(distance))
            {
                // we are in a collision-free end-zone.
                return distance + player * BoardInfo.EndZoneLength;
            }
            else
            {
                // we are out on the competative board where collisions are possible!
                return (BoardInfo.StartPosition(player) + distance - 1) % BoardInfo.Length;
            }
        }
        
        private int CalculateNewPosition(int player, int piece)
        {
            int distance = pieceDistances[player, piece] + CurrentDieRoll;
            if (distance == BoardInfo.GoalDistance)
            {
                return NON_BOARD_POSITION; // goal
            }
            if (distance > BoardInfo.GoalDistance)
            {
                if (Rules.AllowGoalBouncing)
                    distance = Bounce(distance);
                else
                    distance -= CurrentDieRoll; // we can not move, so return where we are currently.
            }
            if (BoardInfo.IsInEndZone(distance))
            {
                return distance + player * BoardInfo.EndZoneLength; // end-zone
            }
            else
            {
                // we are out on the competative board where collisions are possible!
                return (BoardInfo.StartPosition(player) + distance - 1) % BoardInfo.Length;
            }
        }

        private bool CheckVictoryCondition()
        {
            int goal = BoardInfo.GoalDistance;
            if (currentPieces.All(p => p.CurrentDistance == goal))
            {
                Winner = CurrentPlayer;
                // update all PieceInfo so IsInGoal is true:
                for (int i = 0; i < currentPieces.Length; ++i)
                    currentPieces[i] = new PieceInfo(goal, NON_BOARD_POSITION);
                CanMove = false;
                // Game has ended! (no further state changes should be allowed!)
                OnWinnerDeclared();
                return true;
            }
            return false;
        }

        // here we do the heavy lifting! (checking the rules and updating PieceInfo!)
        private void ComputePieceInfo()
        {
            // these are updated by ComputePieceInfo(i)
            InBaseCount = 0;
            InGoalCount = 0;
            CanMove = false;

            // cache'ar resultat här så vi slipper räkna ut base-exit reglerna flera ggr:
            PieceInfo? baseExitInfo = null;

            for (int i = 0; i < PIECE_COUNT; ++i)
                ComputePieceInfo(i);

            // << här tar ComputePieceInfo() metoden slut, koden under är "bara" lokala hjälpmetoder >>

            void ComputePieceInfo(int piece)
            {
                if (IsPieceInBase(piece)) // (distance == 0)
                {
                    ++InBaseCount;
                    if (baseExitInfo == null) // räkna ut och cache'a värdet om det inte finns...
                        ComputeBaseExitInfo();
                    currentPieces[piece] = baseExitInfo.Value; // <-- använd cache'ade värdet.
                }
                else if (IsPieceInGoal(piece)) // (distance == BoardInfo.GoalDistance)
                {
                    ++InGoalCount;
                    currentPieces[piece] = new PieceInfo(BoardInfo.GoalDistance, NON_BOARD_POSITION);
                }
                else if (ALLOW_STACKING)
                {
                    throw new NotImplementedException("Stacking / blocking double-pieces are not currently supported.");
                    // TODO: ...
                }
                else
                {
                    ComputeBoardPieceInfo(piece);
                }
            }

            void ComputeBoardPieceInfo(int piece)
            {
                int oldDistance = pieceDistances[CurrentPlayer, piece];
                int newDistance = oldDistance + CurrentDieRoll;
                int oldPosition = CalculatePosition(CurrentPlayer, piece);
                if (newDistance == BoardInfo.GoalDistance)
                {
                    currentPieces[piece] = new PieceInfo(oldDistance, oldPosition, NON_BOARD_POSITION); // goal!
                    CanMove = true;
                    return; // <---
                }
                if (newDistance > BoardInfo.GoalDistance)
                {
                    if (Rules.AllowGoalBouncing)
                    {
                        newDistance = Bounce(newDistance);
                    }
                    else
                    {
                        currentPieces[piece] = new PieceInfo(oldDistance, oldPosition); // cant move.
                        return; // <---
                    }
                }
                int newPosition = CalculateNewPosition(CurrentPlayer, piece);
                if (LookAtBoard(newPosition) is PlayerPiece pp)
                {
                    //^ the new position collides with something...
                    if (pp.Player == CurrentPlayer)
                    {
                        //^ the new position collides with one of our own pieces
                        if (ALLOW_STACKING)
                        {
                            throw new NotImplementedException("Stacking / blocking double-pieces are not currently supported.");
                            // TODO: ...
                        }
                        else
                        {
                            currentPieces[piece] = new PieceInfo(oldDistance, oldPosition, pp); // cant move.
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
                            currentPieces[piece] = new PieceInfo(oldDistance, oldPosition, newPosition, pp);
                            CanMove = true;
                        }
                    }
                }
                else
                {
                    //^ new position is empty / no collision
                    currentPieces[piece] = new PieceInfo(oldDistance, oldPosition, newPosition);
                    CanMove = true;
                }
            }

            void ComputeBaseExitInfo()
            {
                if (IsBaseRoll)
                {
                    int startPosition = BoardInfo.StartPosition(CurrentPlayer);
                    if (LookAtBoard(startPosition) is PlayerPiece collider)
                    {
                        //^ another piece is occupying our startPosition...
                        if (collider.Player == CurrentPlayer)
                        {
                            //^ we already have a piece on our startingPosition...
                            if (ALLOW_STACKING)
                            {
                                throw new NotImplementedException("Stacking / blocking double-pieces are not currently supported.");
                                // TODO: ...
                            }
                            else
                            {
                                baseExitInfo = new PieceInfo(0, NON_BOARD_POSITION); // we can not move out of base!
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
                                //^ we can kill it!
                                baseExitInfo = new PieceInfo(0, NON_BOARD_POSITION, startPosition, collider);
                                CanMove = true;
                            }
                        }
                    }
                    else
                    {
                        //^ startPosition is empty / no collision...
                        baseExitInfo = new PieceInfo(0, NON_BOARD_POSITION, startPosition);
                        CanMove = true;
                    }
                }
                else // (isBaseRoll == false)
                {
                    baseExitInfo = new PieceInfo(0, NON_BOARD_POSITION); // we can not move out of base!
                }
            }
        }

        // <fields>

        // how far each piece has moved. [player, piece]
        private readonly int[,] pieceDistances;
        private readonly PieceInfo[] currentPieces;
        private readonly Random random = new Random();
    }
}
