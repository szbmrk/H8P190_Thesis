using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokerParty_SharedDLL;

public static class EvaluationHelper
{
    public static PlayerHandInfo[] CreatePlayerHandInfos(TablePlayerCard[] playerCards)
    {
        List<PlayerHandInfo> playerHandInfos = new List<PlayerHandInfo>();
        foreach (TablePlayerCard playerCard in playerCards)
        {
            Card[] cards = new Card[2 + TableManager.Instance.flippedCommunityCards.Length];

            cards[0] = playerCard.turnInfo.cards[0];
            cards[1] = playerCard.turnInfo.cards[1];

            for (int i = 0; i < TableManager.Instance.flippedCommunityCards.Length; i++)
            {
                cards[i + 2] = TableManager.Instance.flippedCommunityCards[i];
            }

            PlayerHandInfo playerHandInfo = new PlayerHandInfo(playerCard.turnInfo.player, cards);
            playerHandInfos.Add(playerHandInfo);
        }

        return playerHandInfos.ToArray();
    }

    public static PlayerHandInfo[] DetermineWinners(TablePlayerCard[] playerSeats)
    {
        PlayerHandInfo[] playerHandInfos = CreatePlayerHandInfos(playerSeats);
        return TexasHoldEm.DetermineWinners(playerHandInfos);
    }
}