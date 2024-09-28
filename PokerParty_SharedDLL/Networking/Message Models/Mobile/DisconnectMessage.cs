using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class DisconnectMessage : ANetworkMessageMobile
    {
        public override NetworkMessageType Type => NetworkMessageType.DisconnectMessage;
    }
}
