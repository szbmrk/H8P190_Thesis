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

        public static HandType EvaluateHand(Card[] hand)
        {
            HandType type = HandType.None;

            int faceValueBitField = EvaluationHelper.GetFaceValueBitField(hand);
            long faceValueCountBitField = EvaluationHelper.GetFaceValueCountBitField(hand);
            type = EvaluationHelper.GetHandTypeByCountOfFaceValues(faceValueCountBitField % 15);

            HandType checkForStraight = EvaluationHelper.CheckForStraight(faceValueBitField);
            if (checkForStraight > type)
                type = checkForStraight;

            HandType checkForFlushes = EvaluationHelper.CheckForFlushes(hand);
            if (checkForFlushes > type)
                type = checkForFlushes;

            return type;
        }

        //in case of a tie we give back an array
        public static Player[] DetermineWinner()
        {
            return null;
        } 
    }
}
