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

        public static Card[][] GetAllPossibleHands(Card[] cards)
        {
            if (cards.Length == 5)
                return new Card[][] { cards };

            List<Card[]> possibleHands = new List<Card[]>();

            int handSize = 5;
            int[] indices = new int[handSize];

            for (int i = 0; i < handSize; i++)
                indices[i] = i;

            while (true)
            {
                Card[] hand = new Card[handSize];
                for (int i = 0; i < handSize; i++)
                    hand[i] = cards[indices[i]];

                possibleHands.Add(hand);

                int k = handSize - 1;
                while (k >= 0 && indices[k] == cards.Length - handSize + k)
                    k--;

                if (k < 0)
                    break;

                indices[k]++;
                for (int j = k + 1; j < handSize; j++)
                    indices[j] = indices[j - 1] + 1;
            }

            return possibleHands.ToArray();
        }

        public static Card[] GetBestHandOfPlayer(Card[][] hands)
        {
            Card[] bestHand = new Card[5];
            HandType bestHandType = HandType.None;
            int bestBreakTieScore = 0;

            foreach (Card[] hand in hands)
            {
                Card[] currentHand = new Card[5];
                Array.Copy(hand, currentHand, 5);
                HandType currentHandType = EvaluateHand(currentHand);
                int currentBreakTieScore = EvaluationHelper.CalculateBreakTieScore(currentHand);

                if (currentHandType >= bestHandType && currentBreakTieScore > bestBreakTieScore)
                {
                    bestHand = currentHand;
                    bestBreakTieScore = currentBreakTieScore;
                    bestHandType = currentHandType;
                }
            }

            return bestHand;
        }

        private static void AssignHandTypes(PlayerHandInfo[] playerHandInfos)
        {
            foreach (PlayerHandInfo playerHandInfo in playerHandInfos)
            {
                playerHandInfo.Type = EvaluateHand(playerHandInfo.Hand);
            }
        }

        public static PlayerHandInfo[] DetermineOrder(PlayerHandInfo[] playerHandInfos)
        {
            AssignHandTypes(playerHandInfos);

            return EvaluationHelper.CreateOrderedPlayerList(playerHandInfos);
        }

        public static PlayerHandInfo[] DetermineWinners(PlayerHandInfo[] playerHandInfos)
        {
            PlayerHandInfo[] orderedPlayers = DetermineOrder(playerHandInfos);
            HandType bestHandType = orderedPlayers[0].Type;
            int bestTieScore = orderedPlayers[0].BreakTieScore;

            PlayerHandInfo[] playersWithBestHand = orderedPlayers
                .TakeWhile(player => player.Type == bestHandType && player.BreakTieScore == bestTieScore)
                .ToArray();

            return playersWithBestHand;
        }
    }
}