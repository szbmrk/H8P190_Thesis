using PokerParty_SharedDLL;
using System.Collections;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public static MatchManager instance;

    // 1% of starting money
    [HideInInspector] public int smallBlindBet;
    // 2% of starting money
    [HideInInspector] public int bigBlindBet;

    private int turnCount = 0;

    private void Awake()
    {
        instance = this;
        smallBlindBet = (int)(Settings.startingMoney * 0.01);
        bigBlindBet = (int)(Settings.startingMoney * 0.02);
    }

    public void SendGameInfoToPlayers()
    {
        foreach (TablePlayerCard playerSeat in TableManager.instance.playerSeats)
        {
            GameInfoMessage gameInfoMessage = new GameInfoMessage
            {
                StartingMoney = Settings.startingMoney,
                SmallBlindAmount = smallBlindBet,
                BigBlindAmount = bigBlindBet,
                IsDealer = playerSeat.isDealer,
                IsSmallBlind = playerSeat.isSmallBlind,
                IsBigBlind = playerSeat.isBigBlind
            };

            int indexInConnections = playerSeat.indexInConnectionsArray;
            ConnectionManager.instance.SendMessageToConnection(ConnectionManager.instance.Connections[indexInConnections], gameInfoMessage);
        }
    }

    public IEnumerator ShowDown()
    {
        TablePlayerCard[] playersStillInGame = TableManager.instance.playerSeats.FindAll(p => !p.TurnInfo.Folded).ToArray();
        PlayerHandInfo[] winners = EvaluationHelper.DetermineWinners(playersStillInGame);
        TableManager.instance.GivePotToWinners(winners);
        TableGUI.instance.RefreshMoneyInPotText(TableManager.instance.moneyInPot);

        if (winners.Length == 1)
            yield return TableGUI.instance.showTurnWinner(winners[0].Player.PlayerName, TexasHoldEm.EvaluateHand(winners[0].Hand));
        else
        {
            string winnerText = "";
            for (int i = 0; i < winners.Length; i++)
            {
                if (i == winners.Length - 1)
                    winnerText += winners[i].Player.PlayerName;
                else
                    winnerText += winners[i].Player.PlayerName + ", ";
            }
            yield return TableGUI.instance.showTurnWinner(winnerText, TexasHoldEm.EvaluateHand(winners[0].Hand));
        }

        turnCount++;
    }
}
