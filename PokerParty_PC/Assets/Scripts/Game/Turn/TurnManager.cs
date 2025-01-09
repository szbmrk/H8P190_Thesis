using System;
using PokerParty_SharedDLL;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
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
    private TablePlayerCard lastPlayerWhoRaised;

    private bool hasAnyoneBetted = false;
    private int highestBet;

    private TablePlayerCard lastPlayerInTurnIfNoOneRaised
    {
        get
        {
            if (turnState == TurnState.PRE_FLOP)
            {
                TablePlayerCard bigBlind = TableManager.Instance.playerSeats.Find(p => p.isBigBlind);
                if (bigBlind.isStillInGame)
                    return bigBlind;
                
                return GetNextPlayerStillInGame(TableManager.Instance.playerSeats.IndexOf(bigBlind));
            }

            TablePlayerCard dealer = TableManager.Instance.playerSeats.Find(p => p.isDealer);
            if (dealer.isStillInGame)
                return dealer;
                
            return GetNextPlayerStillInGame(TableManager.Instance.playerSeats.IndexOf(dealer));
        }
    }

    private void SetNextPlayerInTurn()
    {
        currentPlayerInTurn = GetNextPlayerStillInGame(TableManager.Instance.playerSeats.IndexOf(currentPlayerInTurn));
    }

    public TablePlayerCard GetNextPlayerStillInGame(int currentIndex)
    {
        int nextPlayerIndex;

        if (currentIndex == TableManager.Instance.playerSeats.Count - 1)
            nextPlayerIndex = 0;
        else
            nextPlayerIndex = currentIndex + 1;

        TablePlayerCard nextPlayer = TableManager.Instance.playerSeats[nextPlayerIndex];

        return !nextPlayer.isStillInGame ? GetNextPlayerStillInGame(nextPlayerIndex) : nextPlayer;
    }

    private int playersNeedToCallCount => TableManager.Instance.playerSeats
        .FindAll(player => player.turnInfo.moneyPutInPot < highestBet && player.isStillInGame).Count;

    private int moneyNeededToCall => highestBet - currentPlayerInTurn.turnInfo.moneyPutInPot;

    private int playersStillIngameCount => TableManager.Instance.playerSeats.FindAll(p => p.isStillInGame).Count;

    private void Awake()
    {
        Instance = this;
    }

    public void StartFirstTurn()
    {
        turnState = TurnState.SMALLBLIND_TURN;
        currentPlayerInTurn = TableManager.Instance.playerSeats.Find(p => p.isSmallBlind);
        currentPlayerInTurn.StartTurn();

        PossibleAction[] possibleActions = null;

        if (currentPlayerInTurn.turnInfo.money < MatchManager.Instance.smallBlindBet)
            possibleActions = new PossibleAction[] { PossibleAction.ALL_IN };
        else
            possibleActions = new PossibleAction[] { PossibleAction.SMALL_BLIND_BET };

        SendTurnMessage(possibleActions, MatchManager.Instance.smallBlindBet);
    }

    private void StartSecondTurn()
    {
        turnState = TurnState.BIGBLIND_TURN;
        currentPlayerInTurn.StartTurn();

        PossibleAction[] possibleActions = null;

        if (currentPlayerInTurn.turnInfo.money < MatchManager.Instance.bigBlindBet)
            possibleActions = new PossibleAction[] { PossibleAction.ALL_IN };
        else
            possibleActions = new PossibleAction[] { PossibleAction.BIG_BLIND_BET };

        SendTurnMessage(possibleActions, MatchManager.Instance.bigBlindBet);
    }

    private void StartTurn()
    {
        currentPlayerInTurn.StartTurn();

        PossibleAction[] possibleActions = DeterminePossibleActionsForCurrentPlayer();
        
        SendTurnMessage(possibleActions, moneyNeededToCall);
    }

    private PossibleAction[] DeterminePossibleActionsForCurrentPlayer()
    {
        if (currentPlayerInTurn.turnInfo.money < moneyNeededToCall)
            return new PossibleAction[] { PossibleAction.ALL_IN, PossibleAction.FOLD };

        if (currentPlayerInTurn.turnInfo.money == moneyNeededToCall)
        {
            return new PossibleAction[] { PossibleAction.CALL, PossibleAction.FOLD };
        }
        
        if (moneyNeededToCall == 0)
        {
            if (turnState == TurnState.PRE_FLOP)
                return new PossibleAction[] { PossibleAction.CHECK, PossibleAction.RAISE, PossibleAction.FOLD };
            
            if (!hasAnyoneBetted)
                return new PossibleAction[] { PossibleAction.CHECK, PossibleAction.BET, PossibleAction.FOLD };
            
            return new PossibleAction[] { PossibleAction.CHECK, PossibleAction.RAISE, PossibleAction.FOLD };
        }
        
        return new PossibleAction[] { PossibleAction.CALL, PossibleAction.RAISE, PossibleAction.FOLD };
    }

    private void SendTurnMessage(PossibleAction[] possibleActions, int moneyNeededToCall = 0)
    {
        List<TablePlayerCard> playerSeats = TableManager.Instance.playerSeats;
        
        YourTurnMessage yourTurnMessage = new YourTurnMessage();
        yourTurnMessage.possibleActions = possibleActions;
        yourTurnMessage.moneyNeededToCall = moneyNeededToCall;
        int indexInConnections = currentPlayerInTurn.indexInConnectionsArray;
        ConnectionManager.Instance.SendMessageToConnection(ConnectionManager.Instance.Connections[indexInConnections], yourTurnMessage);

        NotYourTurnMessage notYourTurnMessage = new NotYourTurnMessage();
        notYourTurnMessage.playerInTurn = currentPlayerInTurn.turnInfo.player.PlayerName;

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
        TableManager.Instance.moneyInPot += turnDoneMessage.actionAmount;
        TableGUI.Instance.RefreshMoneyInPotText(TableManager.Instance.moneyInPot);
        UpdateCurrentPlayersTurnInfo(turnDoneMessage);

        if (highestBet < currentPlayerInTurn.turnInfo.moneyPutInPot)
            highestBet = currentPlayerInTurn.turnInfo.moneyPutInPot;

        if (CheckIfTurnIsOver())
            return;

        SetNextPlayerInTurn();

        switch (turnState)
        {
            case TurnState.SMALLBLIND_TURN:
                StartSecondTurn();
                return;
            case TurnState.BIGBLIND_TURN:
                TableManager.Instance.DealCardsToPlayers();
                TableManager.Instance.DealCardsToTable();
                turnState = TurnState.PRE_FLOP;
                StartTurn();
                return;
            default:
                StartTurn();
                break;
        }
    }

    private bool CheckIfCurrentPlayerIsLastPlayerInTurn()
    {
        if (lastPlayerWhoRaised != null && currentPlayerInTurn.Equals(lastPlayerWhoRaised))
            return true;

        return lastPlayerWhoRaised == null && currentPlayerInTurn.Equals(lastPlayerInTurnIfNoOneRaised);
    }

    private bool CheckIfTurnIsOver()
    {
        if (TableManager.Instance.playersInGameCount == 1)
        {
            highestBet = 0;

            StartCoroutine(ShowDown());
            return true;
        }

        if (playersNeedToCallCount == 0 && CheckIfCurrentPlayerIsLastPlayerInTurn())
        {
            lastPlayerWhoRaised = null;
            hasAnyoneBetted = false;

            if (turnState == TurnState.PRE_FLOP)
            {
                turnState = TurnState.FLOP;
                TableManager.Instance.DealFlop();
                SetStartingPlayer();
                StartTurn();
            }
            else if (turnState == TurnState.FLOP)
            {
                turnState = TurnState.TURN;
                TableManager.Instance.DealTurn();
                SetStartingPlayer();
                StartTurn();
            }
            else if (turnState == TurnState.TURN)
            {
                turnState = TurnState.RIVER;
                TableManager.Instance.DealRiver();
                SetStartingPlayer();
                StartTurn();
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

    private void SetStartingPlayer()
    {
        // két játokos esetén a nagyvak kezd
        if (playersStillIngameCount == 2)
        {
            TablePlayerCard bigBlind = TableManager.Instance.playerSeats.Find(p => p.isBigBlind);
            if (bigBlind.isStillInGame)
                currentPlayerInTurn = bigBlind;
            else
                currentPlayerInTurn = GetNextPlayerStillInGame(TableManager.Instance.playerSeats.IndexOf(bigBlind));

            return;
        }

        // több játékos esetén a kisvak kezd
        TablePlayerCard smallBlind = TableManager.Instance.playerSeats.Find(p => p.isSmallBlind);
        if (smallBlind.isStillInGame)
            currentPlayerInTurn = smallBlind;
        else
            currentPlayerInTurn = GetNextPlayerStillInGame(TableManager.Instance.playerSeats.IndexOf(smallBlind));
    }

    private TablePlayerCard GetLastPlayerInGame()
    {
        return TableManager.Instance.playerSeats.FindLast(p => p.isStillInGame);
    }

    private bool CheckIfGameIsOver()
    {
        return TableManager.Instance.playerSeats.Select(p => p.turnInfo.money > 0).Count() == 1;
    }

    private IEnumerator ShowDown()
    {
        yield return MatchManager.Instance.ShowDown();
        if (CheckIfGameIsOver())
        {
            GameManager.Instance.GameOver(GetLastPlayerInGame().turnInfo.player.PlayerName);
            yield break;
        }

        TableManager.Instance.StartNewGame();
        StartFirstTurn();
    }

    private void UpdateCurrentPlayersTurnInfo(TurnDoneMessage turnDoneMessage)
    {
        switch (turnDoneMessage.action)
        {
            case PossibleAction.FOLD:
                currentPlayerInTurn.turnInfo.folded = true;
                currentPlayerInTurn.OutOfTurn();
                return;
            case PossibleAction.BET:
                hasAnyoneBetted = true;
                break;
            case PossibleAction.RAISE:
                lastPlayerWhoRaised = currentPlayerInTurn;
                break;
        }
        
        currentPlayerInTurn.turnInfo.moneyPutInPot += turnDoneMessage.actionAmount;
        currentPlayerInTurn.turnInfo.money -= turnDoneMessage.actionAmount;

        if (currentPlayerInTurn.turnInfo.money == 0)
        {
            currentPlayerInTurn.turnInfo.wentAllIn = true;
            currentPlayerInTurn.OutOfTurn();
        }
        
        currentPlayerInTurn.RefreshMoneyPutIn(currentPlayerInTurn.turnInfo.moneyPutInPot);
    }
}