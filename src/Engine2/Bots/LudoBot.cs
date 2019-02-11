using System;
using Ludo.GameLogic;

namespace Ludo.Bots
{
    public abstract class LudoBot : IDisposable
    {
        // name of the bot implementation - intended to be the same for all instances of the same type.
        public abstract string StaticName { get; }

        public bool IsRegistered => sessionRef.TryGetTarget(out _);
        public int PlayerIndex => IsRegistered ? p_index : -1;

        public void TryMakeMove()
        {
            if (sessionRef.TryGetTarget(out ISession session))
                TryMakeMove(session);
        }

        public void Register(ISession session, int playerIndex, bool tryMakeMove)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));
            if (IsRegistered)
                throw new InvalidOperationException("Cannot have multiple simultanious registrations." +
                    " Unregister from the previous session first if re-registration was intended.");
            SetPlayerIndex(session, playerIndex);
            session.AcceptingInput += Session_AcceptingInput;
            sessionRef.SetTarget(session);
            OnRegistered();
            if (tryMakeMove)
                TryMakeMove(session);
        }


        // does nothing if not registered.
        public void UnRegister()
        {
            if (sessionRef.TryGetTarget(out ISession session))
            {
                session.AcceptingInput -= Session_AcceptingInput;
                sessionRef.SetTarget(null);
            }
        }

        // does nothing if not registered
        public void ChangePlayerIndex(int newPlayerIndex)
        {
            if (sessionRef.TryGetTarget(out ISession session))
                SetPlayerIndex(session, newPlayerIndex);
        }

        public void Dispose() => Dispose(true);

        // </public>  <protected>

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                UnRegister();
        }

        // called after a successful registration has occured.
        protected virtual void OnRegistered() { }

        protected abstract void MakeMove(ISession session);

        // The session is provided here with a weak reference implementation.
        // Thus child classes should avoid holding on to it longer than needed.
        // However it must be stored in a local variable and null-checked before use!
        protected ISession Session
            => sessionRef.TryGetTarget(out ISession s) ? s : null;

        // </protected>  <private>

        private void Session_AcceptingInput(object sender, EventArgs e)
        {
            TryMakeMove((ISession)sender);
        }

        private void TryMakeMove(ISession session)
        {
            if (session.CurrentPlayer == PlayerIndex)
                MakeMove(session);
        }

        private void SetPlayerIndex(ISession session, int playerIndex)
        {
            if (unchecked((uint)playerIndex >= (uint)session.PlayerCount))
                throw new ArgumentOutOfRangeException(nameof(playerIndex));
            p_index = playerIndex;
        }

        // <fields>

        private int p_index;
        private readonly WeakReference<ISession> sessionRef = new WeakReference<ISession>(null);
    }
}
