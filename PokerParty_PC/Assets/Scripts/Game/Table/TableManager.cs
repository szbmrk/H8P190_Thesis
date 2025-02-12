using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using PokerParty_SharedDLL;
using System.Linq;

public class TableManager : MonoBehaviour
{
    public static TableManager instance;

    public List<TablePlayerCard> playerSeats = new List<TablePlayerCard>();
    [SerializeField] private Transform parentForSeats;
    public GameObject playerCardPrefab;

    public List<TableCard> tableCards = new List<TableCard>();
    [SerializeField] private Transform parentForCards;
    public GameObject tableCardPrefab;

    private int playersToConnect = Settings.playerCount;

    private Deck deck;
    public Card[] flippedCommunityCards;

    private int lastDealerIndex;
    private int lastSmallBlindIndex = 1;
    private int lastBigBlindIndex = 2;

    [HideInInspector] public int moneyInPot;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CreateDeck();
    }

    private void CreateDeck()
    {
        deck = new Deck();
        deck.Shuffle();
    }

    public void PlayerLoaded(Player player, int indexOfConnection)
    {
        playersToConnect--;

        TablePlayerCard newPlayer = Instantiate(playerCardPrefab, parentForSeats).GetComponent<TablePlayerCard>();
        newPlayer.gameObject.SetActive(false);

        PlayerTurnInfo playerTurnInfo = new PlayerTurnInfo(player);
        newPlayer.TurnInfo = playerTurnInfo;
        newPlayer.indexInConnectionsArray = indexOfConnection;

        playerSeats.Add(newPlayer);

        if (playersToConnect != 0) return;
        
        AssignRolesAndShuffleTheOrderOfPlayers();
        MatchManager.instance.SendGameInfoToPlayers();
        TurnManager.instance.StartFirstTurn();
        Loader.instance.StopLoading();
        
        Logger.LogToFile("All players loaded");
    }

    public int playersInGameCount => playerSeats
        .FindAll(player => player.isStillInGame).Count;

    private void AssignRolesAndShuffleTheOrderOfPlayers()
    {
        playerSeats = playerSeats.OrderBy(_ => Random.Range(0, 100)).ToList();

        if (playerSeats.Count > 2) 
        {
            playerSeats[0].isDealer = true;
            playerSeats[1].isSmallBlind = true;
            playerSeats[2].isBigBlind = true;
        }
        else
        {
            playerSeats[0].isDealer = true;
            playerSeats[0].isSmallBlind = true;
            playerSeats[1].isBigBlind = true;
        }

        for (int i = 0; i < playerSeats.Count; i++)
        {
            TableGUI.instance.DisplayPlayer(playerSeats[i], i);
        }
    }

    public void DealCardsToPlayers()
    {
        foreach (TablePlayerCard player in playerSeats)
        {
            Card[] cards = TexasHoldEm.DealCardsToPlayer(deck);
            DealCardsMessage dealCardsMessage = new DealCardsMessage();
            player.TurnInfo.Cards = cards;
            dealCardsMessage.Cards = cards;
            int indexInConnections = player.indexInConnectionsArray;
            ConnectionManager.instance.SendMessageToConnection(ConnectionManager.instance.Connections[indexInConnections], dealCardsMessage);
        }
        
        Logger.LogToFile("Cards dealt to players");
    }

    public IEnumerator DealCardsToTable()
    {
        for (int i = 0; i < 5; i++)
        {
            tableCards.Add(Instantiate(tableCardPrefab, parentForCards).GetComponent<TableCard>());
            tableCards[i].card = deck.Draw();
            yield return TableGUI.instance.DisplayCard(tableCards[i], i);
            if (i > 2)
                yield return new WaitForSeconds(0.25f);
        }
    }

    public IEnumerator DealFlop()
    {
        flippedCommunityCards = new Card[3];
        for (int i = 0; i < 3; i++)
        {
            flippedCommunityCards[i] = tableCards[i].card;
            yield return tableCards[i].Flip();
            yield return new WaitForSeconds(0.25f);
        }
        CommunityCardsChangedMessage communityCardsChanged = new CommunityCardsChangedMessage() { CommunityCards = flippedCommunityCards };
        ConnectionManager.instance.SendMessageToAllConnections(communityCardsChanged);
        
        Logger.LogToFile("Flop dealt");
    }

    public IEnumerator DealTurn()
    {
        flippedCommunityCards = new Card[4];
        yield return tableCards[3].Flip();
        flippedCommunityCards[0] = tableCards[0].card;
        flippedCommunityCards[1] = tableCards[1].card;
        flippedCommunityCards[2] = tableCards[2].card;
        flippedCommunityCards[3] = tableCards[3].card;
        CommunityCardsChangedMessage communityCardsChanged = new CommunityCardsChangedMessage() { CommunityCards = flippedCommunityCards };
        ConnectionManager.instance.SendMessageToAllConnections(communityCardsChanged);
        
        Logger.LogToFile("Turn dealt");
    }

    public IEnumerator DealRiver()
    {
        flippedCommunityCards = new Card[5];
        yield return tableCards[4].Flip();
        flippedCommunityCards[0] = tableCards[0].card;
        flippedCommunityCards[1] = tableCards[1].card;
        flippedCommunityCards[2] = tableCards[2].card;
        flippedCommunityCards[3] = tableCards[3].card;
        flippedCommunityCards[4] = tableCards[4].card;
        CommunityCardsChangedMessage communityCardsChanged = new CommunityCardsChangedMessage() { CommunityCards = flippedCommunityCards };
        ConnectionManager.instance.SendMessageToAllConnections(communityCardsChanged);
        
        Logger.LogToFile("River dealt");
    }

    public IEnumerator FlipRemainingCards()
    {
        if (flippedCommunityCards.Length == 5)
            yield break;

        if (flippedCommunityCards.Length == 4)
        {
            yield return DealRiver();
            yield break;
        }
        
        if (flippedCommunityCards.Length == 0)
        {
            yield return DealFlop();
        }
        
        yield return DealTurn();
        yield return new WaitForSeconds(0.25f);
        yield return DealRiver();
    }

    public void PlayerTurnDone(TurnDoneMessage turnDoneMessage)
    {
        playerSeats.Find(p => p.TurnInfo.Player.Equals(turnDoneMessage.Player)).RefreshMoney(turnDoneMessage.NewMoney);
        playerSeats.Find(p => p.TurnInfo.Player.Equals(turnDoneMessage.Player)).TurnDone();
        TurnManager.instance.HandleTurnDone(turnDoneMessage);
    }

    public void GivePotToWinners(PlayerHandInfo[] winners)
    {
        foreach (PlayerHandInfo winner in winners)
        {
            TablePlayerCard winnerCard = playerSeats.Find(p => p.TurnInfo.Player.Equals(winner.Player));
            winnerCard.TurnInfo.Money += moneyInPot / winners.Length;
            winnerCard.TurnInfo.WentAllIn = false;
            winnerCard.RefreshMoney(playerSeats.Find(p => p.TurnInfo.Player.Equals(winner.Player)).TurnInfo.Money);
        }
        moneyInPot = 0;
        
        SendRefreshedMoneyMessages();
        
        Logger.LogToFile("Pot given to winners");
    }

    public void StartNewGame()
    {
        ClearTable();
        ReshuffleDeck();
        ResetAndRotatePlayers();
        RemovePlayersWith0Money();
    }

    public void ClearTable()
    {
        Logger.LogToFile("Clearing table");
        foreach (TableCard card in tableCards)
        {
            Destroy(card.gameObject);
        }
        tableCards.Clear();
    }

    private void ResetAndRotatePlayers()
    {
        ResetPlayers();
        SendNewRoundStartedMessage();
        RotatePlayers();
    }

    private void ResetPlayers()
    {
        Logger.LogToFile("Resetting players");
        lastDealerIndex = playerSeats.IndexOf(playerSeats.Find(p => p.isDealer));
        lastBigBlindIndex = playerSeats.IndexOf(playerSeats.Find(p => p.isBigBlind));
        lastSmallBlindIndex = playerSeats.IndexOf(playerSeats.Find(p => p.isSmallBlind));
        foreach (TablePlayerCard player in playerSeats)
        {
            player.Reset();
        }
    }
    
    private void SendNewRoundStartedMessage()
    {
        NewTurnStartedMessage newTurnStartedMessage = new NewTurnStartedMessage();
        ConnectionManager.instance.SendMessageToAllConnections(newTurnStartedMessage);
    }
    
    private void ReshuffleDeck()
    {
        deck = new Deck();
        deck.Shuffle();
        Logger.LogToFile("Deck reshuffled");
    }
    
    private void RotatePlayers()
    {
        if (playerSeats.Count > 2)
        {
            RotateMultiplePlayers();
        }
        else
        {
            RotateFewerPlayers();
        }

        foreach (TablePlayerCard playerCard in playerSeats)
        {
            playerCard.SetRoleIcons();
        }
        
        Logger.LogToFile("Players rotated");
    }

    private void RotateMultiplePlayers()
    {
        lastDealerIndex = GetNextValidPlayerIndex(lastDealerIndex);
        TablePlayerCard newDealer = playerSeats[lastDealerIndex];
        newDealer.isDealer = true;

        lastSmallBlindIndex = GetNextValidPlayerIndex(lastDealerIndex);
        TablePlayerCard newSmallBlind = playerSeats[lastSmallBlindIndex];
        newSmallBlind.isSmallBlind = true;

        lastBigBlindIndex = GetNextValidPlayerIndex(lastSmallBlindIndex);
        TablePlayerCard newBigBlind = playerSeats[lastBigBlindIndex];
        newBigBlind.isBigBlind = true;
    }

    private void RotateFewerPlayers()
    {
        lastDealerIndex = GetNextValidPlayerIndex(lastDealerIndex);
        TablePlayerCard newDealerAndSmallBlind = playerSeats[lastDealerIndex];
        newDealerAndSmallBlind.isDealer = true;
        newDealerAndSmallBlind.isSmallBlind = true;

        lastBigBlindIndex = GetNextValidPlayerIndex(lastDealerIndex);
        TablePlayerCard newBigBlind = playerSeats[lastBigBlindIndex];
        newBigBlind.isBigBlind = true;
    }

    private int GetNextValidPlayerIndex(int startIndex)
    {
        startIndex++;
        
        if (startIndex > playerSeats.Count - 1)
            startIndex = 0;
        
        int index = startIndex;
        
        TablePlayerCard player = playerSeats[index];

        if (player.isStillInGame) return index;
        
        player = TurnManager.instance.GetNextPlayerStillInGame(index);
        index = playerSeats.IndexOf(player);

        return index;
    }

    public Player GetPLayerByIndexInConnectionsArray(int indexInConnectionsArray)
    {
        return playerSeats.Find(p => p != null && p.indexInConnectionsArray == indexInConnectionsArray).TurnInfo.Player;
    }
    
    public void PlayerDisconnected(Player player)
    {
        TablePlayerCard playerCard = playerSeats.Find(p => p.TurnInfo.Player.Equals(player));
        bool wasCurrentlyInTurn = playerCard.TurnInfo.Player.Equals(TurnManager.instance.currentPlayerInTurn.TurnInfo.Player);
        playerCard.Disconnected();
        playerCard.TurnInfo.Money = 0;
        playerSeats.Remove(playerCard);
        
        if (wasCurrentlyInTurn) return;
        
        if (playerSeats.Count == 1)
        {
            GameManager.instance.GameOver(playerSeats[0]);
        }
    }

    public void RemovePlayersWith0Money()
    {
        List<TablePlayerCard> playersThatAreOut = playerSeats.Select(p => p).Where(p => !p.isStillInGame).ToList();
        SendGameOverMessages(playersThatAreOut);
        
        foreach (TablePlayerCard player in playersThatAreOut)
        {
            Logger.LogToFile($"{player.TurnInfo.Player.PlayerName} is out of money");
            player.OutOfGame();
            playerSeats.Remove(player);
            Logger.LogToFile($"{player.TurnInfo.Player.PlayerName} removed from the game");
        }
    }

    private void SendRefreshedMoneyMessages()
    {
        foreach (TablePlayerCard player in playerSeats)
        {
            RefreshedMoneyMessage refreshedMoneyMessage = new RefreshedMoneyMessage
            {
                NewMoney = player.TurnInfo.Money
            };
            ConnectionManager.instance.SendMessageToConnection(
                ConnectionManager.instance.Connections[player.indexInConnectionsArray], refreshedMoneyMessage);
        }
    }

    private void SendGameOverMessages(List<TablePlayerCard> players)
    {
        foreach (TablePlayerCard player in players)
        {
            GameOverMessage gameOverMessage = new GameOverMessage
            {
                Place = playerSeats.Count - players.Count + 1
            };

            ConnectionManager.instance.SendMessageToConnection(
                ConnectionManager.instance.Connections[player.indexInConnectionsArray], gameOverMessage);   
        }
    }
}
