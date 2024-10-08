using PokerParty_SharedDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class TurnManager
{
    private static int turnCount = 0;

    public static void StartFirstTurn()
    {
        turnCount++;
        TableManager.Instance.playerSeats[1].StartTurn();
        PossibleAction[] possibleActions = new PossibleAction[] { PossibleAction.SMALL_BLIND_BET };
        SendTurnMessage(1, possibleActions);
    }

    public static void SendTurnMessage(int playerIndex, PossibleAction[] possibleActions, int previousBet = 0)
    {
        List<TablePlayerCard> playerSeats = TableManager.Instance.playerSeats;

        YourTurnMessage yourTurnMessage = new YourTurnMessage();
        yourTurnMessage.PossibleActions = possibleActions;
        yourTurnMessage.PreviousBet = previousBet;
        int indexInConnections =  playerSeats[playerIndex].indexInConnectionsArray;
        ConnectionManager.Instance.SendMessageToConnection(ConnectionManager.Instance.Connections[indexInConnections], yourTurnMessage);

        NotYourTurnMessage notYourTurnMessage = new NotYourTurnMessage();
        notYourTurnMessage.PlayerInTurn = playerSeats[playerIndex].assignedPlayer.playerName;

        for (int i = 0; i < playerSeats.Count; i++)
        {
            if (i != playerIndex)
            {
                indexInConnections = playerSeats[i].indexInConnectionsArray;
                ConnectionManager.Instance.SendMessageToConnection(ConnectionManager.Instance.Connections[indexInConnections], notYourTurnMessage);
            }
        }
    }
}
