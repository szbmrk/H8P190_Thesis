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
    private TablePlayerCard currentPlayerInTurn;
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
            return new PossibleAction[] { PossibleAction.AllIn, PossibleAction.Fold };

        if (currentPlayerInTurn.TurnInfo.Money == moneyNeededToCall)
            return new PossibleAction[] { PossibleAction.Call, PossibleAction.Fold };

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

    public void HandleTurnDone(TurnDoneMessage turnDoneMessage)
    {
        if (!turnDoneMessage.Player.Equals(currentPlayerInTurn.TurnInfo.Player))
            return;

        if (PauseMenu.instance.isPaused)
        {
            PauseMenu.instance.turnDoneMessageReceivedDuringPause = turnDoneMessage;
            return;
        }
        
        TableManager.instance.moneyInPot += turnDoneMessage.ActionAmount;
        TableGUI.instance.RefreshMoneyInPotText(TableManager.instance.moneyInPot);
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
                TableManager.instance.DealCardsToTable();
                turnState = TurnState.PreFlop;
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

        if (playersNeedToCallCount == 0 && CheckIfCurrentPlayerIsLastPlayerInTurn())
        {
            lastPlayerWhoRaised = null;
            hasAnyoneBetted = false;

            if (turnState == TurnState.PreFlop)
            {
                turnState = TurnState.Flop;
                TableManager.instance.DealFlop();
                SetStartingPlayer();
                StartTurn();
            }
            else if (turnState == TurnState.Flop)
            {
                turnState = TurnState.Turn;
                TableManager.instance.DealTurn();
                SetStartingPlayer();
                StartTurn();
            }
            else if (turnState == TurnState.Turn)
            {
                turnState = TurnState.River;
                TableManager.instance.DealRiver();
                SetStartingPlayer();
                StartTurn();
            }
            else if (turnState == TurnState.River)
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
            currentPlayerInTurn = bigBlind.isStillInGame ? bigBlind : GetNextPlayerStillInGame(TableManager.instance.playerSeats.IndexOf(bigBlind));

            return;
        }

        // több játékos esetén a kisvak kezd
        TablePlayerCard smallBlind = TableManager.instance.playerSeats.Find(p => p.isSmallBlind);
        currentPlayerInTurn = smallBlind.isStillInGame ? smallBlind : GetNextPlayerStillInGame(TableManager.instance.playerSeats.IndexOf(smallBlind));
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
        
                
        currentPlayerInTurn.SetLastActionText($"{turnDoneMessage.Action} {turnDoneMessage.ActionAmount} $");
        
        if (turnDoneMessage.Action == PossibleAction.Check)
            currentPlayerInTurn.SetLastActionText($"CHECK");
    }
}