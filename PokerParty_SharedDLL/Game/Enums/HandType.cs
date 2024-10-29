using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerParty_SharedDLL
{
    public enum HandType
    {
        RoyalFlush = 10,
        StraightFlush = 9,
        FourOfAKind = 8,
        FullHouse = 7,
        Flush = 6,
        Straight = 5,
        ThreeOfAKind = 4,
        TwoPair = 3,
        OnePair = 2,
        HighCard = 1,
        None = 0
    }
}
