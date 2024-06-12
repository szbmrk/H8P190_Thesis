using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public abstract class ANetworkMessage
    {
        public Player player;
        public abstract NetworkMessageType Type { get; }
    }
}
