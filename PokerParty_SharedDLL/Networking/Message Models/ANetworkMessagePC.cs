using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public abstract class ANetworkMessagePC
    {
        public abstract NetworkMessageType Type { get; }
    }
}
