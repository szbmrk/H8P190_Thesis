using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public abstract class ANetworkMessageMobile
    {
        public string playerName;
        public abstract NetworkMessageType Type { get; }
    }
}
