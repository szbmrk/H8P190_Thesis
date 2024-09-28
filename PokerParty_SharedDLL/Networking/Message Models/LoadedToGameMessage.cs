using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class LoadedToGameMessage : ANetworkMessage
    {
        public override NetworkMessageType Type => NetworkMessageType.LoadedToGameMessage;
    }
}
