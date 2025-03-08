using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class LobbyIsFullMessage : ANetworkMessagePc
    {
        public override NetworkMessageType Type => NetworkMessageType.LobbyIsFullMessage;
    }
}