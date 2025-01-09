
using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class GameStartedMessage : ANetworkMessagePc
    {
        public override NetworkMessageType Type => NetworkMessageType.GameStartedMessage;
    }
}
