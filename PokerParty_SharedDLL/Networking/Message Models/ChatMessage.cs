using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class ChatMessage : ANetworkMessage
    {
        public string message;

        public override NetworkMessageType Type => NetworkMessageType.ChatMessage;
    }
}
