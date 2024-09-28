using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class LoadedToGameMessage : ANetworkMessageMobile
    {
        public override NetworkMessageType Type => NetworkMessageType.LoadedToGameMessage;
    }
}
