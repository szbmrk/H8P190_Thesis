
using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class GameStartedMessage : ANetworkMessagePC
    {
        public override NetworkMessageType Type => NetworkMessageType.GameStartedMessage;
    }
}
