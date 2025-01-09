using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public abstract class ANetworkMessageMobile
    {
        public Player Player;
        public abstract NetworkMessageType Type { get; }
    }
}
