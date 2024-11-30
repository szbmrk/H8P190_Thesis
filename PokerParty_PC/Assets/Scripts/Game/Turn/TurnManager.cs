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
                if (bigBlind.IsStillInGame)
                    return bigBlind;
                else
                    return GetNextPlayerStillInGame(TableManager.Instance.playerSeats.IndexOf(bigBlind));
            }
         
            TablePlayerCard dealer = TableManager.Instance.playerSeats.Find(p => p.isDealer);
            if (dealer.IsStillInGame)
                return dealer;
            else
                return GetNextPlayerStillInGame(TableManager.Instance.playerSeats.IndexOf(dealer));
        }
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

        if (!nextPlayer.IsStillInGame)
            return GetNextPlayerStillInGame(nextPlayerIndex);

        return nextPlayer;
    }

    private int PlayersNeedToCallCount => TableManager.Instance.playerSeats
        .FindAll(player => player.turnInfo.moneyPutInPot < highestBet && player.IsStillInGame).Count;

    private int MoneyNeededToCall => highestBet - currentPlayerInTurn.turnInfo.moneyPutInPot;

    private int PlayersStillIngameCount => TableManager.Instance.playerSeats.FindAll(p => p.IsStillInGame).Count;

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

    public void StartSecondTurn()
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

    public void StartTurn()
    {
        currentPlayerInTurn.StartTurn();

        PossibleAction[] possibleActions = null;
        if (highestBet == 0)
            possibleActions = new PossibleAction[] { PossibleAction.BET, PossibleAction.FOLD };
        else if (currentPlayerInTurn.turnInfo.money < MoneyNeededToCall)
            possibleActions = new PossibleAction[] { PossibleAction.FOLD, PossibleAction.ALL_IN };
        else if (currentPlayerInTurn.turnInfo.money >= MoneyNeededToCall)
        {
            possibleActions = new PossibleAction[] { PossibleAction.CALL, PossibleAction.FOLD };
            if (currentPlayerInTurn.turnInfo.money > MoneyNeededToCall)
                possibleActions = new PossibleAction[] { PossibleAction.CALL, PossibleAction.RAISE, PossibleAction.FOLD };
        }
        else if (currentPlayerInTurn.turnInfo.moneyPutInPot == highestBet)
            possibleActions = new PossibleAction[] { PossibleAction.CHECK };

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
        if (TableManager.Instance.PlayersInGameCount == 1)
        {
            TableManager.Instance.moneyInPot += moneyInTurn;
            moneyInTurn = 0;
            highestBet = 0;

            StartCoroutine(ShowDown());
            return true;
        }

        if (PlayersNeedToCallCount == 0 && currentPlayerInTurn.Equals(lastPlayerInTurn))
        {
            TableManager.Instance.moneyInPot += moneyInTurn;
            TableGUI.Instance.RefreshMoneyInPotText(TableManager.Instance.moneyInPot);
            moneyInTurn = 0;
            highestBet = 0;

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
        if (PlayersStillIngameCount == 2) 
        {
            TablePlayerCard bigBlind = TableManager.Instance.playerSeats.Find(p => p.isBigBlind);
            if (bigBlind.IsStillInGame)
                currentPlayerInTurn = bigBlind;
            else
                currentPlayerInTurn = GetNextPlayerStillInGame(TableManager.Instance.playerSeats.IndexOf(bigBlind));

            return;
        }

        // több játékos esetén a kisvak kezd
        TablePlayerCard smallBlind = TableManager.Instance.playerSeats.Find(p => p.isSmallBlind);
        if (smallBlind.IsStillInGame)
            currentPlayerInTurn = smallBlind;
        else
            currentPlayerInTurn = GetNextPlayerStillInGame(TableManager.Instance.playerSeats.IndexOf(smallBlind));
    }

    private TablePlayerCard GetLastPlayerInGame()
    {
        return TableManager.Instance.playerSeats.FindLast(p => p.IsStillInGame);
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
            GameManager.Instance.GameOver(GetLastPlayerInGame().turnInfo.player.playerName);
            yield break;
        }

        TableManager.Instance.StartNewGame();
        StartFirstTurn();
    }

    private void StartNewTurn()
    {
        TablePlayerCard smallBlind = TableManager.Instance.playerSeats.Find(p => p.isSmallBlind);
        if (smallBlind.IsStillInGame)
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