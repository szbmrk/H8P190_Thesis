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
    public static TurnManager instance;

    private TurnState turnState = TurnState.SMALLBLIND_TURN;
    private TablePlayerCard currentPlayerInTurn;
    private TablePlayerCard lastPlayerWhoRaised;

    private bool hasAnyoneBetted;
    private int highestBet;

    private TablePlayerCard lastPlayerInTurnIfNoOneRaised
    {
        get
        {
            if (turnState == TurnState.PRE_FLOP)
            {
                TablePlayerCard bigBlind = TableManager.instance.playerSeats.Find(p => p.isBigBlind);
                return bigBlind.isStillInGame ? bigBlind : GetNextPlayerStillInGame(TableManager.instance.playerSeats.IndexOf(bigBlind));
            }

            TablePlayerCard dealer = TableManager.instance.playerSeats.Find(p => p.isDealer);
            return dealer.isStillInGame ? dealer : GetNextPlayerStillInGame(TableManager.instance.playerSeats.IndexOf(dealer));
        }
    }

    private void SetNextPlayerInTurn()
    {
        currentPlayerInTurn = GetNextPlayerStillInGame(TableManager.instance.playerSeats.IndexOf(currentPlayerInTurn));
    }

    public TablePlayerCard GetNextPlayerStillInGame(int currentIndex)
    {
        int nextPlayerIndex;

        if (currentIndex == TableManager.instance.playerSeats.Count - 1)
            nextPlayerIndex = 0;
        else
            nextPlayerIndex = currentIndex + 1;

        TablePlayerCard nextPlayer = TableManager.instance.playerSeats[nextPlayerIndex];

        return !nextPlayer.isStillInGame ? GetNextPlayerStillInGame(nextPlayerIndex) : nextPlayer;
    }

    private int playersNeedToCallCount => TableManager.instance.playerSeats
        .FindAll(player => player.TurnInfo.MoneyPutInPot < highestBet && player.isStillInGame).Count;

    private int moneyNeededToCall => highestBet - currentPlayerInTurn.TurnInfo.MoneyPutInPot;

    private int playersStillInGameCount => TableManager.instance.playerSeats.FindAll(p => p.isStillInGame).Count;

    private void Awake()
    {
        instance = this;
    }

    public void StartFirstTurn()
    {
        turnState = TurnState.SMALLBLIND_TURN;
        currentPlayerInTurn = TableManager.instance.playerSeats.Find(p => p.isSmallBlind);
        currentPlayerInTurn.StartTurn();

        PossibleAction[] possibleActions = null;

        if (currentPlayerInTurn.TurnInfo.Money < MatchManager.instance.smallBlindBet)
            possibleActions = new PossibleAction[] { PossibleAction.ALL_IN };
        else
            possibleActions = new PossibleAction[] { PossibleAction.SMALL_BLIND_BET };

        SendTurnMessage(possibleActions, MatchManager.instance.smallBlindBet);
    }

    private void StartSecondTurn()
    {
        turnState = TurnState.BIGBLIND_TURN;
        currentPlayerInTurn.StartTurn();

        PossibleAction[] possibleActions = null;

        if (currentPlayerInTurn.TurnInfo.Money < MatchManager.instance.bigBlindBet)
            possibleActions = new PossibleAction[] { PossibleAction.ALL_IN };
        else
            possibleActions = new PossibleAction[] { PossibleAction.BIG_BLIND_BET };

        SendTurnMessage(possibleActions, MatchManager.instance.bigBlindBet);
    }

    private void StartTurn()
    {
        currentPlayerInTurn.StartTurn();

        PossibleAction[] possibleActions = DeterminePossibleActionsForCurrentPlayer();
        
        SendTurnMessage(possibleActions, moneyNeededToCall);
    }

    private PossibleAction[] DeterminePossibleActionsForCurrentPlayer()
    {
        if (currentPlayerInTurn.TurnInfo.Money < moneyNeededToCall)
            return new PossibleAction[] { PossibleAction.ALL_IN, PossibleAction.FOLD };

        if (currentPlayerInTurn.TurnInfo.Money == moneyNeededToCall)
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
        List<TablePlayerCard> playerSeats = TableManager.instance.playerSeats;
        
        YourTurnMessage yourTurnMessage = new YourTurnMessage
        {
            possibleActions = possibleActions,
            moneyNeededToCall = moneyNeededToCall
        };
        int indexInConnections = currentPlayerInTurn.indexInConnectionsArray;
        ConnectionManager.instance.SendMessageToConnection(ConnectionManager.instance.Connections[indexInConnections], yourTurnMessage);

        NotYourTurnMessage notYourTurnMessage = new NotYourTurnMessage
        {
            playerInTurn = currentPlayerInTurn.TurnInfo.Player.PlayerName
        };

        foreach (TablePlayerCard player in playerSeats)
        {
            if (!player.Equals(currentPlayerInTurn))
            {
                indexInConnections = player.indexInConnectionsArray;
                ConnectionManager.instance.SendMessageToConnection(ConnectionManager.instance.Connections[indexInConnections], notYourTurnMessage);
            }
        }
    }

    public void HandleTurnDone(TurnDoneMessage turnDoneMessage)
    {
        TableManager.instance.moneyInPot += turnDoneMessage.actionAmount;
        TableGUI.instance.RefreshMoneyInPotText(TableManager.instance.moneyInPot);
        UpdateCurrentPlayersTurnInfo(turnDoneMessage);

        if (highestBet < currentPlayerInTurn.TurnInfo.MoneyPutInPot)
            highestBet = currentPlayerInTurn.TurnInfo.MoneyPutInPot;

        if (CheckIfTurnIsOver())
            return;

        SetNextPlayerInTurn();

        switch (turnState)
        {
            case TurnState.SMALLBLIND_TURN:
                StartSecondTurn();
                return;
            case TurnState.BIGBLIND_TURN:
                TableManager.instance.DealCardsToPlayers();
                TableManager.instance.DealCardsToTable();
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
        if (TableManager.instance.playersInGameCount == 1)
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
                TableManager.instance.DealFlop();
                SetStartingPlayer();
                StartTurn();
            }
            else if (turnState == TurnState.FLOP)
            {
                turnState = TurnState.TURN;
                TableManager.instance.DealTurn();
                SetStartingPlayer();
                StartTurn();
            }
            else if (turnState == TurnState.TURN)
            {
                turnState = TurnState.RIVER;
                TableManager.instance.DealRiver();
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
        if (playersStillInGameCount == 2)
        {
            TablePlayerCard bigBlind = TableManager.instance.playerSeats.Find(p => p.isBigBlind);
            if (bigBlind.isStillInGame)
                currentPlayerInTurn = bigBlind;
            else
                currentPlayerInTurn = GetNextPlayerStillInGame(TableManager.instance.playerSeats.IndexOf(bigBlind));

            return;
        }

        // több játékos esetén a kisvak kezd
        TablePlayerCard smallBlind = TableManager.instance.playerSeats.Find(p => p.isSmallBlind);
        if (smallBlind.isStillInGame)
            currentPlayerInTurn = smallBlind;
        else
            currentPlayerInTurn = GetNextPlayerStillInGame(TableManager.instance.playerSeats.IndexOf(smallBlind));
    }

    private TablePlayerCard GetLastPlayerInGame()
    {
        return TableManager.instance.playerSeats.FindLast(p => p.isStillInGame);
    }

    private bool CheckIfGameIsOver()
    {
        return TableManager.instance.playerSeats.Select(p => p.TurnInfo.Money > 0).Count() == 1;
    }

    private IEnumerator ShowDown()
    {
        yield return MatchManager.instance.ShowDown();
        if (CheckIfGameIsOver())
        {
            GameManager.instance.GameOver(GetLastPlayerInGame().TurnInfo.Player.PlayerName);
            yield break;
        }

        TableManager.instance.StartNewGame();
        StartFirstTurn();
    }

    private void UpdateCurrentPlayersTurnInfo(TurnDoneMessage turnDoneMessage)
    {
        switch (turnDoneMessage.action)
        {
            case PossibleAction.FOLD:
                currentPlayerInTurn.TurnInfo.Folded = true;
                currentPlayerInTurn.OutOfTurn();
                return;
            case PossibleAction.BET:
                hasAnyoneBetted = true;
                break;
            case PossibleAction.RAISE:
                lastPlayerWhoRaised = currentPlayerInTurn;
                break;
        }
        
        currentPlayerInTurn.TurnInfo.MoneyPutInPot += turnDoneMessage.actionAmount;
        currentPlayerInTurn.TurnInfo.Money -= turnDoneMessage.actionAmount;

        if (currentPlayerInTurn.TurnInfo.Money == 0)
        {
            currentPlayerInTurn.TurnInfo.WentAllIn = true;
            currentPlayerInTurn.OutOfTurn();
        }
        
        currentPlayerInTurn.RefreshMoneyPutIn(currentPlayerInTurn.TurnInfo.MoneyPutInPot);
    }
}