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

        public override bool Equals(object obj)
        {
            if (obj is null || !(obj is Player))
            {
                return false;
            }
            return ((Player)obj).PlayerName == PlayerName;
        }
    }
}