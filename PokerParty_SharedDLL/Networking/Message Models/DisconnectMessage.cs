using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class DisconnectMessage : ANetworkMessage
    {
        public override NetworkMessageType Type => NetworkMessageType.DisconnectMessage;
    }
}
