using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class ReadyMessage : ANetworkMessageMobile
    {
        public bool IsReady;
        public override NetworkMessageType Type => NetworkMessageType.ReadyMessage;
    }
}
