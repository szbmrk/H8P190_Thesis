using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class ConnectionMessage : ANetworkMessageMobile
    {
        public override NetworkMessageType Type => NetworkMessageType.ConnectionMessage;
    }

}
