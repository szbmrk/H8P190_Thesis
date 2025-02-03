using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class GameAlreadyStartedMessage : ANetworkMessagePc
    {
        public override NetworkMessageType Type => NetworkMessageType.GameAlreadyStartedMessage;
    }
}