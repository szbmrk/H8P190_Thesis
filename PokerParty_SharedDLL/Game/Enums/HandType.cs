using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerParty_SharedDLL
{
    public enum HandType
    {
        RoyalFlush = 100,
        StraightFlush = 90,
        FourOfAKind = 80,
        FullHouse = 70,
        Flush = 60,
        Straight = 50,
        ThreeOfAKind = 40,
        TwoPair = 30,
        OnePair = 20,
        HighCard = 10,
        None = 0
    }
}
