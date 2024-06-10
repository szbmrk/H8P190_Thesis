using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class ChatMessage
    {
        public Player player;
        public string message;
    }
}
