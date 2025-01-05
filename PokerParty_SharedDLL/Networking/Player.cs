using System;

namespace PokerParty_SharedDLL
{
    [Serializable]
    public class Player
    {
        public string PlayerName { get; set; }
        
        public Player(string playerName)
        {
            PlayerName = playerName;
        }
    }
}