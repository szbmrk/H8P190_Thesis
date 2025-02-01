using System;
using System.Collections.Generic;
using System.Linq;

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
            {
                return new Card[][] { cards };
            }

            List<Card[]> possibleHands = new List<Card[]>();
            int handSize = 5;
            int n = cards.Length;

            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    for (int k = j + 1; k < n; k++)
                    {
                        for (int l = k + 1; l < n; l++)
                        {
                            for (int m = l + 1; m < n; m++)
                            {
                                Card[] hand = new Card[handSize];
                                hand[0] = cards[i];
                                hand[1] = cards[j];
                                hand[2] = cards[k];
                                hand[3] = cards[l];
                                hand[4] = cards[m];
                                possibleHands.Add(hand);
                            }
                        }
                    }
                }
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
                
                if (currentHandType > bestHandType || (currentHandType == bestHandType && currentBreakTieScore > bestBreakTieScore))
                {
                    bestHandType = currentHandType;
                    bestBreakTieScore = currentBreakTieScore;
                    Array.Copy(currentHand, bestHand, 5);
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

        private static PlayerHandInfo[] DetermineOrder(PlayerHandInfo[] playerHandInfos)
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