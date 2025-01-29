using System.Collections.Generic;
using PokerParty_SharedDLL;

public static class EvaluationHelper
{
    private static PlayerHandInfo[] CreatePlayerHandInfos(TablePlayerCard[] playerCards)
    {
        List<PlayerHandInfo> playerHandInfos = new List<PlayerHandInfo>();
        foreach (TablePlayerCard playerCard in playerCards)
        {
            Card[] cards = new Card[7];

            cards[0] = playerCard.TurnInfo.Cards[0];
            cards[1] = playerCard.TurnInfo.Cards[1];

            for (int i = 0; i < TableManager.instance.tableCards.Count; i++)
            {
                cards[i + 2] = TableManager.instance.tableCards[i].card;
            }

            Card[] hand = TexasHoldEm.GetBestHandOfPlayer(TexasHoldEm.GetAllPossibleHands(cards));
            
            PlayerHandInfo playerHandInfo = new PlayerHandInfo(playerCard.TurnInfo.Player, hand);
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