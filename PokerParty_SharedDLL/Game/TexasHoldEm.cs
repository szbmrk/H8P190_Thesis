using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerParty_SharedDLL
{
    public static class TexasHoldEm
    {
        public static Card[] DealCardsToPlayer(Deck deck)
        {
            Card[] cards = new Card[2];
            cards[0] = deck.Draw();
            cards[1] = deck.Draw();
            return cards;
        }

        public static Card[] DealFlop(Deck deck)
        {
            Card[] cards = new Card[3];
            cards[0] = deck.Draw();
            cards[1] = deck.Draw();
            cards[2] = deck.Draw();
            return cards;
        }

        public static int EvaluateHand(Card[] hand)
        {
            return 0;
        }
    }
}
