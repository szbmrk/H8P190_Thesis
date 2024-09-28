using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public abstract class ANetworkMessageMobile
    {
        public Player player;
        public abstract NetworkMessageType Type { get; }
    }
}
