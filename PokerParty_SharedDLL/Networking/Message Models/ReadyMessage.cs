using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class ReadyMessage : ANetworkMessage
    {
        public bool isReady;
        public override NetworkMessageType Type => NetworkMessageType.ReadyMessage;
    }
}
