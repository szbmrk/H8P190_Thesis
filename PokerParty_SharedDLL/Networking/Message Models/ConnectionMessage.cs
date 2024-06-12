using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class ConnectionMessage : ANetworkMessage
    {
        public override NetworkMessageType Type => NetworkMessageType.ConnectionMessage;
    }

}
