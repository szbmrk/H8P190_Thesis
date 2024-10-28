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
            //hand is assumed to be 7 cards
            //hand[0] and hand[1] are the player's cards
            //hand[2] through hand[6] are the community cards

            return 0;
        }
    }
}
