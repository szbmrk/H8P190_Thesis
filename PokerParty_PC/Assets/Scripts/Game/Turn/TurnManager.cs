using PokerParty_SharedDLL;
using System.Collections.Generic;
using UnityEngine;

public enum TurnState
{
    SMALLBLIND_TURN,
    BIGBLIND_TURN,
    PRE_FLOP,
    FLOP,
    TURN,
    RIVER
}

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    private TurnState turnState = TurnState.SMALLBLIND_TURN;
    private TablePlayerCard currentPlayerInTurn;

    private int moneyInTurn = 0;
    private int highestBet = 0;

    private TablePlayerCard lastPlayerInTurn
    {
        get
        {
            if (turnState == TurnState.PRE_FLOP) // big blind
            {
                if (IsPlayerStillInGame(TableManager.Instance.playerSeats[2]))
                    return TableManager.Instance.playerSeats[2];
                else
                    return GetNextPlayerStillInGame(2);
            }
         
            // dealer
            if (IsPlayerStillInGame(TableManager.Instance.playerSeats[0]))
                return TableManager.Instance.playerSeats[0];
            else
                return GetNextPlayerStillInGame(0);
        }
    }

    private bool IsPlayerStillInGame(TablePlayerCard player)
    {
        return !player.turnInfo.folded && !player.turnInfo.wentAllIn;
    }

    private void SetNextPlayerInTurn()
    {
        currentPlayerInTurn = GetNextPlayerStillInGame(TableManager.Instance.playerSeats.IndexOf(currentPlayerInTurn)); ;
    }

    private TablePlayerCard GetNextPlayerStillInGame(int currentIndex)
    {
        TablePlayerCard nextPlayer = null;
        int nextPlayerIndex = -1;

        if (currentIndex == TableManager.Instance.playerSeats.Count - 1)
            nextPlayerIndex = 0;
        else
            nextPlayerIndex = currentIndex + 1;

        nextPlayer = TableManager.Instance.playerSeats[nextPlayerIndex];

        if (!IsPlayerStillInGame(nextPlayer))
            return GetNextPlayerStillInGame(nextPlayerIndex);

        return nextPlayer;
    }

    private int PlayersInGameCount => TableManager.Instance.playerSeats
        .FindAll(player => IsPlayerStillInGame(player)).Count;

    private int PlayersNeedToCallCount => TableManager.Instance.playerSeats
        .FindAll(player => player.turnInfo.moneyPutInPot < highestBet && IsPlayerStillInGame(player)).Count;

    private int MoneyNeededToCall => highestBet - currentPlayerInTurn.turnInfo.moneyPutInPot;

    private void Awake()
    {
        Instance = this;
    }

    public void StartFirstTurn()
    {
        currentPlayerInTurn = TableManager.Instance.playerSeats[1];
        currentPlayerInTurn.StartTurn();

        PossibleAction[] possibleActions = null;

        if (currentPlayerInTurn.turnInfo.money < TableManager.Instance.smallBlindBet)
            possibleActions = new PossibleAction[] { PossibleAction.FOLD, PossibleAction.ALL_IN };
        else
            possibleActions = new PossibleAction[] { PossibleAction.SMALL_BLIND_BET };

        SendTurnMessage(possibleActions, TableManager.Instance.smallBlindBet);
    }

    public void StartSecondTurn()
    {
        turnState = TurnState.BIGBLIND_TURN;
        currentPlayerInTurn.StartTurn();

        PossibleAction[] possibleActions = null;

        if (currentPlayerInTurn.turnInfo.money < TableManager.Instance.bigBlindBet)
            possibleActions = new PossibleAction[] { PossibleAction.FOLD, PossibleAction.ALL_IN };
        else
            possibleActions = new PossibleAction[] { PossibleAction.BIG_BLIND_BET };

        SendTurnMessage(possibleActions, TableManager.Instance.bigBlindBet);
    }

    public void StartTurn()
    {
        currentPlayerInTurn.StartTurn();

        PossibleAction[] possibleActions = null;
        if (highestBet == 0)
            possibleActions = new PossibleAction[] { PossibleAction.BET, PossibleAction.FOLD };
        else if (currentPlayerInTurn.turnInfo.money < MoneyNeededToCall)
            possibleActions = new PossibleAction[] { PossibleAction.FOLD, PossibleAction.ALL_IN };
        else if (currentPlayerInTurn.turnInfo.moneyPutInPot < MoneyNeededToCall)
            possibleActions = new PossibleAction[] { PossibleAction.CALL, PossibleAction.RAISE, PossibleAction.FOLD };
        else if (currentPlayerInTurn.turnInfo.moneyPutInPot == MoneyNeededToCall)
            possibleActions = new PossibleAction[] { PossibleAction.CHECK, PossibleAction.RAISE };

        SendTurnMessage(possibleActions, MoneyNeededToCall);
    }

    public void SendTurnMessage(PossibleAction[] possibleActions, int moneyNeededToCall = 0)
    {
        List<TablePlayerCard> playerSeats = TableManager.Instance.playerSeats;

        YourTurnMessage yourTurnMessage = new YourTurnMessage();
        yourTurnMessage.possibleActions = possibleActions;
        yourTurnMessage.moneyNeededToCall = moneyNeededToCall;
        int indexInConnections = currentPlayerInTurn.indexInConnectionsArray;
        ConnectionManager.Instance.SendMessageToConnection(ConnectionManager.Instance.Connections[indexInConnections], yourTurnMessage);

        NotYourTurnMessage notYourTurnMessage = new NotYourTurnMessage();
        notYourTurnMessage.playerInTurn = currentPlayerInTurn.turnInfo.player.playerName;

        foreach (TablePlayerCard player in playerSeats)
        {
            if (!player.Equals(currentPlayerInTurn))
            {
                indexInConnections = player.indexInConnectionsArray;
                ConnectionManager.Instance.SendMessageToConnection(ConnectionManager.Instance.Connections[indexInConnections], notYourTurnMessage);
            }
        }
    }

    public void HandleTurnDone(TurnDoneMessage turnDoneMessage)
    {
        moneyInTurn += turnDoneMessage.actionAmount;
        UpdateCurrentPlayersTurnInfo(turnDoneMessage);

        if (highestBet < currentPlayerInTurn.turnInfo.moneyPutInPot)
            highestBet = currentPlayerInTurn.turnInfo.moneyPutInPot;

        CheckIfTurnIsOver();

        SetNextPlayerInTurn();

        if (turnState == TurnState.SMALLBLIND_TURN)
        {
            StartSecondTurn();
            return;
        }

        if (turnState == TurnState.BIGBLIND_TURN)
        {
            TableManager.Instance.DealCardsToPlayers();
            TableManager.Instance.DealCardsToTable();
            turnState = TurnState.PRE_FLOP;
            StartTurn();
            return;
        }

        if (turnState == TurnState.PRE_FLOP || turnState == TurnState.FLOP || turnState == TurnState.TURN || turnState == TurnState.RIVER)
        {
            StartTurn();
        }
    }

    private void CheckIfTurnIsOver()
    {
        if (PlayersInGameCount == 1)
        {
            TableManager.Instance.moneyInPot += moneyInTurn;
            GameManager.Instance.GameOver();
            return;
        }

        if (PlayersNeedToCallCount == 0 && currentPlayerInTurn.Equals(lastPlayerInTurn))
        {
            TableManager.Instance.moneyInPot += moneyInTurn;
            TableGUI.Instance.RefreshMoneyInPotText(TableManager.Instance.moneyInPot);
            moneyInTurn = 0;
            highestBet = 0;

            if (IsPlayerStillInGame(TableManager.Instance.playerSeats[1]))
                currentPlayerInTurn = TableManager.Instance.playerSeats[1];
            else
                currentPlayerInTurn = GetNextPlayerStillInGame(1);

            if (turnState == TurnState.PRE_FLOP)
            {
                turnState = TurnState.FLOP;
                TableManager.Instance.DealFlop();
            }
            else if (turnState == TurnState.FLOP)
            {
                turnState = TurnState.TURN;
                TableManager.Instance.DealTurn();
            }
            else if (turnState == TurnState.TURN)
            {
                turnState = TurnState.RIVER;
                TableManager.Instance.DealRiver();
            }
            else if (turnState == TurnState.RIVER)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    private void UpdateCurrentPlayersTurnInfo(TurnDoneMessage turnDoneMessage)
    {
        if (turnDoneMessage.action == PossibleAction.FOLD)
        {
            currentPlayerInTurn.turnInfo.folded = true;
            currentPlayerInTurn.OutOfTurn();
            return;
        }

        if (turnDoneMessage.action == PossibleAction.ALL_IN)
        {
            currentPlayerInTurn.turnInfo.wentAllIn = true;
            currentPlayerInTurn.turnInfo.moneyPutInPot += turnDoneMessage.actionAmount;
            currentPlayerInTurn.RefreshMoneyPutIn(currentPlayerInTurn.turnInfo.moneyPutInPot);
            currentPlayerInTurn.OutOfTurn();
            return;
        }

        currentPlayerInTurn.turnInfo.moneyPutInPot += turnDoneMessage.actionAmount;
        currentPlayerInTurn.RefreshMoneyPutIn(currentPlayerInTurn.turnInfo.moneyPutInPot);
    }
}