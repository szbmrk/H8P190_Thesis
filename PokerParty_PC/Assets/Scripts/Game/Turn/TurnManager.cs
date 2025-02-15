using PokerParty_SharedDLL;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TurnState
{
    SmallBlindTurn,
    BigBlindTurn,
    PreFlop,
    Flop,
    Turn,
    River
}

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    private TurnState turnState = TurnState.SmallBlindTurn;
    public TablePlayerCard currentPlayerInTurn;
    private TablePlayerCard lastPlayerWhoRaised;

    private bool hasAnyoneBetted;
    private int highestBet;

    private TablePlayerCard lastPlayerInTurnIfNoOneRaised
    {
        get
        {
            if (turnState == TurnState.PreFlop)
            {
                TablePlayerCard bigBlind = TableManager.instance.playerSeats.Find(p => p.isBigBlind);
                return bigBlind != null && bigBlind.isStillInGame ? bigBlind : GetNextPlayerStillInGame(TableManager.instance.playerSeats.IndexOf(bigBlind));
            }

            if (playersStillInGameCount > 2)
            {
                TablePlayerCard dealer = TableManager.instance.playerSeats.Find(p => p.isDealer);
                return dealer != null && dealer.isStillInGame ? dealer : GetNextPlayerStillInGame(TableManager.instance.playerSeats.IndexOf(dealer));
            }
            
            TablePlayerCard smallBlind = TableManager.instance.playerSeats.Find(p => p.isSmallBlind);
            return smallBlind != null && smallBlind.isStillInGame ? smallBlind : GetNextPlayerStillInGame(TableManager.instance.playerSeats.IndexOf(smallBlind));
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

    private TablePlayerCard GetPreviousPlayerStillInGame(int currentIndex)
    {
        int previousPlayerIndex;
        
        if (currentIndex == 0)
            previousPlayerIndex = TableManager.instance.playerSeats.Count - 1;
        else
            previousPlayerIndex = currentIndex - 1;
        
        TablePlayerCard previousPlayer = TableManager.instance.playerSeats[previousPlayerIndex];
        
        return !previousPlayer.isStillInGame ? GetPreviousPlayerStillInGame(previousPlayerIndex) : previousPlayer;
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
        turnState = TurnState.SmallBlindTurn;
        currentPlayerInTurn = TableManager.instance.playerSeats.Find(p => p.isSmallBlind);
        currentPlayerInTurn.StartTurn();

        PossibleAction[] possibleActions = null;

        if (currentPlayerInTurn.TurnInfo.Money < MatchManager.instance.smallBlindBet)
            possibleActions = new PossibleAction[] { PossibleAction.AllIn };
        else
            possibleActions = new PossibleAction[] { PossibleAction.SmallBlindBet };

        SendTurnMessage(possibleActions, MatchManager.instance.smallBlindBet);
        Logger.LogToFile("First turn started");
    }

    private void StartSecondTurn()
    {
        turnState = TurnState.BigBlindTurn;
        currentPlayerInTurn.StartTurn();

        PossibleAction[] possibleActions = null;

        if (currentPlayerInTurn.TurnInfo.Money < MatchManager.instance.bigBlindBet)
            possibleActions = new PossibleAction[] { PossibleAction.AllIn };
        else
            possibleActions = new PossibleAction[] { PossibleAction.BigBlindBet };

        SendTurnMessage(possibleActions, MatchManager.instance.bigBlindBet);
        Logger.LogToFile("Second turn started");
    }

    private void StartTurn()
    {
        currentPlayerInTurn.StartTurn();

        PossibleAction[] possibleActions = DeterminePossibleActionsForCurrentPlayer();
        
        SendTurnMessage(possibleActions, moneyNeededToCall);
        Logger.LogToFile("Turn started");
    }

    private PossibleAction[] DeterminePossibleActionsForCurrentPlayer()
    {
        if (currentPlayerInTurn.TurnInfo.Money <= moneyNeededToCall)
            return new PossibleAction[] { PossibleAction.AllIn, PossibleAction.Fold };

        if (moneyNeededToCall != 0)
            return new PossibleAction[] { PossibleAction.Call, PossibleAction.Raise, PossibleAction.Fold };
        
        if (turnState == TurnState.PreFlop)
            return new PossibleAction[] { PossibleAction.Check, PossibleAction.Raise, PossibleAction.Fold };
            
        if (!hasAnyoneBetted)
            return new PossibleAction[] { PossibleAction.Check, PossibleAction.Bet, PossibleAction.Fold };
            
        return new PossibleAction[] { PossibleAction.Check, PossibleAction.Raise, PossibleAction.Fold };

    }

    private void SendTurnMessage(PossibleAction[] possibleActions, int moneyNeededToCall = 0)
    {
        List<TablePlayerCard> playerSeats = TableManager.instance.playerSeats;
        
        YourTurnMessage yourTurnMessage = new YourTurnMessage
        {
            PossibleActions = possibleActions,
            MoneyNeededToCall = moneyNeededToCall
        };
        
        int indexInConnections = currentPlayerInTurn.indexInConnectionsArray;
        ConnectionManager.instance.SendMessageToConnection(ConnectionManager.instance.Connections[indexInConnections], yourTurnMessage);
        Logger.LogToFile($"Sent turn message to {currentPlayerInTurn.TurnInfo.Player.PlayerName}");

        NotYourTurnMessage notYourTurnMessage = new NotYourTurnMessage
        {
            PlayerInTurn = currentPlayerInTurn.TurnInfo.Player.PlayerName
        };

        foreach (TablePlayerCard player in playerSeats)
        {
            if (player.Equals(currentPlayerInTurn)) continue;
            
            indexInConnections = player.indexInConnectionsArray;
            ConnectionManager.instance.SendMessageToConnection(ConnectionManager.instance.Connections[indexInConnections], notYourTurnMessage);
        }
    }

    public void HandleTurnDone(TurnDoneMessage turnDoneMessage, bool disconnected = false)
    {
        if (!disconnected && (turnDoneMessage == null || turnDoneMessage.Player == null || currentPlayerInTurn == null))
            return;

        if (!disconnected && !turnDoneMessage.Player.Equals(currentPlayerInTurn.TurnInfo.Player))
            return;

        if (PauseMenu.instance.isPaused)
        {
            PauseMenu.instance.turnDoneMessageReceivedDuringPause = turnDoneMessage;
            return;
        }
        
        if (!disconnected)
            Logger.LogToFile($"{turnDoneMessage.Player.PlayerName} {turnDoneMessage.Action} {turnDoneMessage.ActionAmount}$");
        
        TableManager.instance.moneyInPot += turnDoneMessage.ActionAmount;
        TableGUI.instance.RefreshMoneyInPotText(TableManager.instance.moneyInPot);
        
        if (!disconnected)
            UpdateCurrentPlayersTurnInfo(turnDoneMessage);

        if (highestBet < currentPlayerInTurn.TurnInfo.MoneyPutInPot)
            highestBet = currentPlayerInTurn.TurnInfo.MoneyPutInPot;

        if (CheckIfTurnIsOver())
            return;

        SetNextPlayerInTurn();

        switch (turnState)
        {
            case TurnState.SmallBlindTurn:
                StartSecondTurn();
                return;
            case TurnState.BigBlindTurn:
                TableManager.instance.DealCardsToPlayers();
                StartCoroutine(HandleDealCardsToTableTransition());
                return;
            default:
                StartTurn();
                break;
        }
    }
    
    private IEnumerator HandleDealCardsToTableTransition()
    {
        yield return TableManager.instance.StartCoroutine(TableManager.instance.DealCardsToTable());
        turnState = TurnState.PreFlop;
        StartTurn();
    }

    private bool CheckIfCurrentPlayerIsLastPlayerInTurn()
    {
        if (lastPlayerWhoRaised != null && currentPlayerInTurn.Equals(lastPlayerWhoRaised))
            return true;

        if (lastPlayerWhoRaised != null && lastPlayerWhoRaised.TurnInfo.WentAllIn)
            return currentPlayerInTurn.Equals(GetPreviousPlayerStillInGame(TableManager.instance.playerSeats.IndexOf(lastPlayerWhoRaised)))
                || currentPlayerInTurn.Equals(TableManager.instance.playerSeats[TableManager.instance.playerSeats.IndexOf(lastPlayerWhoRaised) - 1]);
        
        return lastPlayerWhoRaised == null && currentPlayerInTurn.Equals(lastPlayerInTurnIfNoOneRaised);
    }

    private bool CheckIfTurnIsOver()
    {
        if ((playersNeedToCallCount == 0 && TableManager.instance.playersInGameCount == 1) || playersStillInGameCount == 0)
        {
            StartCoroutine(ShowDown());
            return true;
        }

        if (playersNeedToCallCount != 0 || !CheckIfCurrentPlayerIsLastPlayerInTurn()) return false;
        lastPlayerWhoRaised = null;
        hasAnyoneBetted = false;

        switch (turnState)
        {
            case TurnState.PreFlop:
                turnState = TurnState.Flop;
                StartCoroutine(HandleFlopTransition());
                break;
            case TurnState.Flop:
                turnState = TurnState.Turn;
                StartCoroutine(HandleTurnTransition());
                break;
            case TurnState.Turn:
                turnState = TurnState.River;
                StartCoroutine(HandleRiverTransition());
                break;
            case TurnState.River:
                StartCoroutine(ShowDown());
                return true;
        }
        return true;

    }
    
    private IEnumerator HandleFlopTransition()
    {
        GameManager.instance.HideWaitingFor();
        yield return TableManager.instance.StartCoroutine(TableManager.instance.DealFlop());
        SetStartingPlayer();
        StartTurn();
    }
    
    private IEnumerator HandleTurnTransition()
    {
        GameManager.instance.HideWaitingFor();
        yield return TableManager.instance.StartCoroutine(TableManager.instance.DealTurn());
        SetStartingPlayer();
        StartTurn();
    }
    
    private IEnumerator HandleRiverTransition()
    {
        GameManager.instance.HideWaitingFor();
        yield return TableManager.instance.StartCoroutine(TableManager.instance.DealRiver());
        SetStartingPlayer();
        StartTurn();
    }

    private void SetStartingPlayer()
    {
        if (playersStillInGameCount == 2)
        {
            TablePlayerCard bigBlind = TableManager.instance.playerSeats.Find(p => p.isBigBlind);
            currentPlayerInTurn = bigBlind != null && bigBlind.isStillInGame ? bigBlind : GetNextPlayerStillInGame(TableManager.instance.playerSeats.IndexOf(bigBlind));
            
            return;
        }

        TablePlayerCard smallBlind = TableManager.instance.playerSeats.Find(p => p.isSmallBlind);
        currentPlayerInTurn = smallBlind != null && smallBlind.isStillInGame ? smallBlind : GetNextPlayerStillInGame(TableManager.instance.playerSeats.IndexOf(smallBlind));
        
        if (currentPlayerInTurn.Equals(lastPlayerInTurnIfNoOneRaised))
            currentPlayerInTurn = GetNextPlayerStillInGame(TableManager.instance.playerSeats.IndexOf(currentPlayerInTurn));
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
        Logger.LogToFile("Showdown started");
        
        highestBet = 0;
        yield return MatchManager.instance.ShowDown();
        TableManager.instance.StartNewGame();

        if (CheckIfGameIsOver())
        {
            GameManager.instance.GameOver(GetLastPlayerInGame());
            yield break;
            
        }
        StartFirstTurn();
    }

    private void UpdateCurrentPlayersTurnInfo(TurnDoneMessage turnDoneMessage)
    {
        switch (turnDoneMessage.Action)
        {
            case PossibleAction.Fold:
                currentPlayerInTurn.Fold();
                return;
            case PossibleAction.Bet:
                hasAnyoneBetted = true;
                lastPlayerWhoRaised = currentPlayerInTurn;
                break;
            case PossibleAction.Raise:
                lastPlayerWhoRaised = currentPlayerInTurn;
                break;
        }
        
        currentPlayerInTurn.TurnInfo.MoneyPutInPot += turnDoneMessage.ActionAmount;
        currentPlayerInTurn.TurnInfo.Money -= turnDoneMessage.ActionAmount;

        if (currentPlayerInTurn.TurnInfo.Money == 0)
        {
            currentPlayerInTurn.AllIn();
            
            if (currentPlayerInTurn.TurnInfo.MoneyPutInPot > highestBet)
                lastPlayerWhoRaised = currentPlayerInTurn;
            
            return;
        }
        
                
        currentPlayerInTurn.SetLastActionText($"{turnDoneMessage.Action} {turnDoneMessage.ActionAmount}$");
        
        if (turnDoneMessage.Action == PossibleAction.Check)
            currentPlayerInTurn.SetLastActionText($"CHECK");
    }
}