using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class GameUnpausedMessage : ANetworkMessagePc
    {
        public override NetworkMessageType Type => NetworkMessageType.GameUnpausedMessage;
    }
}