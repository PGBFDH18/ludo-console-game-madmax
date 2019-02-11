using System;

namespace Ludo.GameLogic
{
    public class LudoRuleException : InvalidOperationException
    {
        public LudoRuleException()
        { }

        public LudoRuleException(string message) : base(message)
        { }
    }
}
