using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public abstract class ANetworkMessagePc
    {
        public abstract NetworkMessageType Type { get; }
    }
}
