using PokerParty_SharedDLL;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            if (turnState == TurnState.PRE_FLOP)
            {
                TablePlayerCard bigBlind = TableManager.Instance.playerSeats.Find(p => p.isBigBlind);
                if (IsPlayerStillInGame(bigBlind))
                    return bigBlind;
                else
                    return GetNextPlayerStillInGame(TableManager.Instance.playerSeats.IndexOf(bigBlind));
            }
         
            TablePlayerCard dealer = TableManager.Instance.playerSeats.Find(p => p.isDealer);
            if (IsPlayerStillInGame(dealer))
                return dealer;
            else
                return GetNextPlayerStillInGame(TableManager.Instance.playerSeats.IndexOf(dealer));
        }
    }

    public bool IsPlayerStillInGame(TablePlayerCard player)
    {
        return !player.turnInfo.folded && !player.turnInfo.wentAllIn && player.turnInfo.money > 0;
    }

    private void SetNextPlayerInTurn()
    {
        currentPlayerInTurn = GetNextPlayerStillInGame(TableManager.Instance.playerSeats.IndexOf(currentPlayerInTurn)); ;
    }

    public TablePlayerCard GetNextPlayerStillInGame(int currentIndex)
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
            possibleActions = new PossibleAction[] { PossibleAction.ALL_IN };
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
            possibleActions = new PossibleAction[] { PossibleAction.ALL_IN };
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

        if (CheckIfTurnIsOver())
            return;

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

    private bool CheckIfTurnIsOver()
    {
        if (PlayersInGameCount == 1)
        {
            TableManager.Instance.moneyInPot += moneyInTurn;
            moneyInTurn = 0;
            highestBet = 0;

            StartCoroutine(TableManager.Instance.ShowDown());
            return true;
        }

        if (PlayersNeedToCallCount == 0 && currentPlayerInTurn.Equals(lastPlayerInTurn))
        {
            TableManager.Instance.moneyInPot += moneyInTurn;
            moneyInTurn = 0;
            highestBet = 0;

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
                StartCoroutine(ShowDown());
                return true;
            }
            return true;
        }

        return false;
    }

    private TablePlayerCard GetLastPlayerInGame()
    {
        return TableManager.Instance.playerSeats.FindLast(p => IsPlayerStillInGame(p));
    }

    private bool CheckIfGameIsOver()
    {
        return TableManager.Instance.playerSeats.Select(p => p.turnInfo.money > 0).Count() == 1;
    }

    private IEnumerator ShowDown()
    {
        yield return TableManager.Instance.ShowDown();
        if (CheckIfGameIsOver())
        {
            GameManager.Instance.GameOver(GetLastPlayerInGame().turnInfo.player.playerName);
            yield break;
        }

        TableManager.Instance.ResetAndRotatePlayers();
        StartNewTurn();
    }

    private void StartNewTurn()
    {
        TablePlayerCard smallBlind = TableManager.Instance.playerSeats.Find(p => p.isSmallBlind);
        if (IsPlayerStillInGame(smallBlind))
            currentPlayerInTurn = smallBlind;
        else
            currentPlayerInTurn = GetNextPlayerStillInGame(TableManager.Instance.playerSeats.IndexOf(smallBlind));
        StartTurn();
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