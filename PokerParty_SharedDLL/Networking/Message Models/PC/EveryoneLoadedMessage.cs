
using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class EveryoneLoadedMessage : ANetworkMessagePC
    {
        public override NetworkMessageType Type => NetworkMessageType.EveryoneLoadedMessage;
    }
}
