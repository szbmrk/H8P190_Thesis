using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class GamePausedMessage : ANetworkMessagePc
    {
        public override NetworkMessageType Type => NetworkMessageType.GamePausedMessage;
    }
}