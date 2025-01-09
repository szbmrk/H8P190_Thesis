using System.Collections.Generic;
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
        Loader.instance.StartLoading();
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

        if (playersToConnect == 0)
        {
            AssignRolesAndShuffleTheOrderOfPlayers();
            MatchManager.instance.SendGameInfoToPlayers();
            TurnManager.instance.StartFirstTurn();
            Loader.instance.StopLoading();
        }
    }

    public int playersInGameCount => playerSeats
        .FindAll(player => player.isStillInGame).Count;

    private void AssignRolesAndShuffleTheOrderOfPlayers()
    {
        playerSeats = playerSeats.OrderBy(_ => Random.Range(0, 100)).ToList();

        playerSeats[0].isDealer = true;
        playerSeats[1].isSmallBlind = true;
        playerSeats[2].isBigBlind = true;

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
    }

    public void DealCardsToTable()
    {
        for (int i = 0; i < 5; i++)
        {
            tableCards.Add(Instantiate(tableCardPrefab, parentForCards).GetComponent<TableCard>());
            tableCards[i].card = deck.Draw();
            TableGUI.instance.DisplayCard(tableCards[i], i);
        }
    }

    public void DealFlop()
    {
        flippedCommunityCards = new Card[3];
        for (int i = 0; i < 3; i++)
        {
            flippedCommunityCards[i] = tableCards[i].card;
            tableCards[i].Flip();
        }
        CommunityCardsChanged communityCardsChanged = new CommunityCardsChanged() { CommunityCards = flippedCommunityCards };
        ConnectionManager.instance.SendMessageToAllConnections(communityCardsChanged);
    }

    public void DealTurn()
    {
        flippedCommunityCards = new Card[4];
        tableCards[3].Flip();
        flippedCommunityCards[0] = tableCards[0].card;
        flippedCommunityCards[1] = tableCards[1].card;
        flippedCommunityCards[2] = tableCards[2].card;
        flippedCommunityCards[3] = tableCards[3].card;
        CommunityCardsChanged communityCardsChanged = new CommunityCardsChanged() { CommunityCards = flippedCommunityCards };
        ConnectionManager.instance.SendMessageToAllConnections(communityCardsChanged);
    }

    public void DealRiver()
    {
        tableCards[4].Flip();
        flippedCommunityCards = new Card[5];
        flippedCommunityCards[0] = tableCards[0].card;
        flippedCommunityCards[1] = tableCards[1].card;
        flippedCommunityCards[2] = tableCards[2].card;
        flippedCommunityCards[3] = tableCards[3].card;
        flippedCommunityCards[4] = tableCards[4].card;
        CommunityCardsChanged communityCardsChanged = new CommunityCardsChanged() { CommunityCards = flippedCommunityCards };
        ConnectionManager.instance.SendMessageToAllConnections(communityCardsChanged);
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
    }

    public void StartNewGame()
    {
        ClearTable();
        ReshuffleDeck();
        ResetAndRotatePlayers();
        RemovePlayersWith0Money();
    }

    private void ClearTable()
    {
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
    }
    
    private void RotatePlayers()
    {
        if (playerSeats.Count >= 3)
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
    }

    private void RotateMultiplePlayers()
    {
        lastDealerIndex = GetNextValidPlayerIndex(lastDealerIndex);
        TablePlayerCard newDealer = playerSeats[lastDealerIndex];
        newDealer.isDealer = true;

        lastSmallBlindIndex = GetNextValidPlayerIndex(lastDealerIndex + 1);
        TablePlayerCard newSmallBlind = playerSeats[lastSmallBlindIndex];
        newSmallBlind.isSmallBlind = true;

        lastBigBlindIndex = GetNextValidPlayerIndex(lastSmallBlindIndex + 1);
        TablePlayerCard newBigBlind = playerSeats[lastBigBlindIndex];
        newBigBlind.isBigBlind = true;
    }

    private void RotateFewerPlayers()
    {
        lastDealerIndex = GetNextValidPlayerIndex(lastDealerIndex);
        TablePlayerCard newDealerAndSmallBlind = playerSeats[lastDealerIndex];
        newDealerAndSmallBlind.isDealer = true;
        newDealerAndSmallBlind.isSmallBlind = true;

        lastBigBlindIndex = GetNextValidPlayerIndex(lastDealerIndex + 1);
        TablePlayerCard newBigBlind = playerSeats[lastBigBlindIndex];
        newBigBlind.isBigBlind = true;
    }

    private int GetNextValidPlayerIndex(int startIndex)
    {
        int index = startIndex % playerSeats.Count;
        TablePlayerCard player = playerSeats[index];

        if (!player.isStillInGame)
        {
            player = TurnManager.instance.GetNextPlayerStillInGame(index);
            index = playerSeats.IndexOf(player);
        }

        return index;
    }

    public void PlayerDisconnected(Player player)
    {
        TablePlayerCard playerCard = playerSeats.Find(p => p.TurnInfo.Player.Equals(player));
        playerCard.Disconnected();
        playerCard.TurnInfo.Money = 0;
    }

    private void RemovePlayersWith0Money()
    {
        List<TablePlayerCard> playersThatAreOut = playerSeats.Select(p => p).Where(p => !p.isStillInGame).ToList();
        SendGameOverMessages(playersThatAreOut);
        
        foreach (TablePlayerCard player in playersThatAreOut)
        {
            player.OutOfGame();
            playerSeats.Remove(player);
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
            ConnectionManager.instance.SendMessageToConnection(
                ConnectionManager.instance.Connections[player.indexInConnectionsArray], new GameOverMessage());   
        }
    }
}
