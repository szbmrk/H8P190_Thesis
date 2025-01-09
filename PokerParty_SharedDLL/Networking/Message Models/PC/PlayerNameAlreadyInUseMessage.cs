using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class PlayerNameAlreadyInUseMessage : ANetworkMessagePc
    {
        public override NetworkMessageType Type => NetworkMessageType.PlayerNameAlreadyInUseMessage;
    }
}