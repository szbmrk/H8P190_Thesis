using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class ReadyMessage
    {
        public Player player;
        public bool isReady;
    }
}
