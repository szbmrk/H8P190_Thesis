using PokerParty_SharedDLL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance;

    // 1% of starting money
    [HideInInspector] public int smallBlindBet;
    // 2% of starting money
    [HideInInspector] public int bigBlindBet;

    private int turnCount = 0;

    private void Awake()
    {
        Instance = this;
        smallBlindBet = (int)(Settings.StartingMoney * 0.01);
        bigBlindBet = (int)(Settings.StartingMoney * 0.02);
    }

    public void SendGameInfoToPlayers()
    {
        for (int i = 0; i < TableManager.Instance.playerSeats.Count; i++)
        {
            TablePlayerCard playerSeat = TableManager.Instance.playerSeats[i];
            GameInfoMessage gameInfoMessage = new GameInfoMessage();
            gameInfoMessage.startingMoney = Settings.StartingMoney;
            gameInfoMessage.smallBlindAmount = smallBlindBet;
            gameInfoMessage.bigBlindAmount = bigBlindBet;
            gameInfoMessage.isDealer = playerSeat.isDealer;
            gameInfoMessage.isSmallBlind = playerSeat.isSmallBlind;
            gameInfoMessage.isBigBlind = playerSeat.isBigBlind;

            int indexInConnections = playerSeat.indexInConnectionsArray;
            ConnectionManager.Instance.SendMessageToConnection(ConnectionManager.Instance.Connections[indexInConnections], gameInfoMessage);
        }
    }

    public IEnumerator ShowDown()
    {
        TablePlayerCard[] playersStillInGame = TableManager.Instance.playerSeats.FindAll(p => !p.turnInfo.folded).ToArray();
        PlayerHandInfo[] winners = EvaluationHelper.DetermineWinners(playersStillInGame);
        TableManager.Instance.GivePotToWinners(winners);
        TableGUI.Instance.RefreshMoneyInPotText(TableManager.Instance.moneyInPot);

        if (winners.Length == 1)
            yield return TableGUI.Instance.showTurnWinner(winners[0].Player.playerName);
        else
        {
            string winnerText = "";
            for (int i = 0; i < winners.Length; i++)
            {
                if (i == winners.Length - 1)
                    winnerText += winners[i].Player.playerName;
                else
                    winnerText += winners[i].Player.playerName + ", ";
            }
            yield return TableGUI.Instance.showTurnWinner(winnerText);
        }

        turnCount++;
    }
}
