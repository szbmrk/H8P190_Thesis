using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class ChatMessage : ANetworkMessageMobile
    {
        public string Message;

        public override NetworkMessageType Type => NetworkMessageType.ChatMessage;
    }
}
