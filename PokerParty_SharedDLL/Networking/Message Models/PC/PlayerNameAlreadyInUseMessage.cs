using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class PlayerNameAlreadyInUseMessage : ANetworkMessagePC
    {
        public override NetworkMessageType Type => NetworkMessageType.PlayerNameAlreadyInUseMessage;
    }
}