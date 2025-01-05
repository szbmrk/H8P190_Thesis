using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerParty_SharedDLL
{
    public class PlayerHandInfo
    {
        public string PlayerName { get; set; }
        public Card[] Hand { get; set; }
        public HandType Type { get; set; }
        public int BreakTieScore { get; set; }

        public PlayerHandInfo(string playerName, Card[] hand)
        {
            PlayerName = playerName;
            Hand = hand;
        }
    }
}
