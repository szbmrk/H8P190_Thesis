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
        
        Logger.Log("Game info sent to players");
    }

    public IEnumerator ShowDown()
    {
        yield return TableManager.instance.FlipRemainingCards();
        
        TablePlayerCard[] playersStillInGame = TableManager.instance.playerSeats.FindAll(p => !p.TurnInfo.Folded).ToArray();
        PlayerHandInfo[] winners = EvaluationHelper.DetermineWinners(playersStillInGame);
        
        TablePlayerCard[] winnerPlayerSeats = new TablePlayerCard[winners.Length];
        for (int i = 0; i < winners.Length; i++)
        {
            winnerPlayerSeats[i] = TableManager.instance.playerSeats.Find(p => p.TurnInfo.Player.Equals(winners[i].Player));
        }
        
        TableManager.instance.GivePotToWinners(winners);

        if (winners.Length == 1)
            yield return TableGUI.instance.ShowTurnWinner(winners[0].Player.PlayerName, winners[0].Type, winnerPlayerSeats);
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
            yield return TableGUI.instance.ShowTurnWinner(winnerText, winners[0].Type, winnerPlayerSeats);
        }

        TableGUI.instance.RefreshMoneyInPotText(TableManager.instance.moneyInPot);
        turnCount++;
    }
}
