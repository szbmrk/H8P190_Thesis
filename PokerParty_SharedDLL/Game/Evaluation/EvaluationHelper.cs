using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerParty_SharedDLL
{
    public static class EvaluationHelper
    {
        public static int GetFaceValueBitField(Card[] hand)
        {
            int bitField = 0;

            foreach (Card card in hand)
            {
                int bitPosition = card.value;
                bitField |= (1 << bitPosition);
            }

            return bitField;
        }

        public static long GetFaceValueCountBitField(Card[] hand)
        {
            long bitField = 0;

            int[] counts = new int[13];

            foreach (Card card in hand)
            {
                int index = card.value - 2;
                counts[index]++;
            }

            for (int i = 0; i < counts.Length; i++)
            {
                int countBitField = 0;

                for (int j = 0; j <= counts[i]; j++)
                {
                    countBitField = (1 << j) - 1;
                }

                bitField |= (long)countBitField << (i * 4) + 8;
            }

            return bitField;
        }

        public static HandType GetHandTypeByCountOfFaceValues(long mod15Res)
        {
            if (mod15Res == 1) return HandType.FourOfAKind;
            if (mod15Res == 10) return HandType.FullHouse;
            if (mod15Res == 9) return HandType.ThreeOfAKind;
            if (mod15Res == 7) return HandType.TwoPair;
            if (mod15Res == 6) return HandType.OnePair;
            if (mod15Res == 5) return HandType.HighCard;

            return HandType.None;
        }

        public static HandType CheckForStraight(int bitField) 
        {
            int divisor = bitField & -bitField;

            if (bitField / divisor == 31)
                return HandType.Straight;

            //check for ace-low straight
            if (bitField == 0b100000000111100)
                return HandType.Straight;

            return HandType.None; 
        }

        public static HandType CheckForFlushes(Card[] hand)
        {
            int bitField = GetFaceValueBitField(hand);
            if (bitField == 0b111110000000000)
                return HandType.RoyalFlush;

            HandType type = HandType.Flush;
            string suit = hand[0].suit;

            foreach (Card card in hand)
            {
                if (card.suit != suit)
                    type = HandType.None;
            }

            if (CheckForStraight(bitField) == HandType.Straight && type == HandType.Flush)
                type = HandType.StraightFlush;

            return type;
        }

        public static int[] BreakTies(params Card[][] hands)
        {
            int[] scores = new int[hands.Length];

            for (int i = 0; i < hands.Length; i++)
            {
                scores[i] = CalculateBreakTieScore(hands[i]);
            }

            scores.OrderByDescending(score => score);

            return scores;
        }

        private static int CalculateBreakTieScore(Card[] hand)
        {
            Card[] sortedHand = hand
                .GroupBy(card => card.value)
                .OrderByDescending(group => group.Count())
                .ThenByDescending(group => group.Key)
                .SelectMany(group => group)
                .ToArray();

            int score = 0;

            for (int i = 0; i < sortedHand.Length; i++)
            {
                score |= sortedHand[i].value << (16 - (i * 4));
            }

            return score;
        }
    }
}
