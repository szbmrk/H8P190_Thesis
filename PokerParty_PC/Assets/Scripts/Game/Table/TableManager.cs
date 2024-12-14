using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PokerParty_SharedDLL;
using System.Linq;

public class TableManager : MonoBehaviour
{
    public static TableManager Instance;

    public List<TablePlayerCard> playerSeats = new List<TablePlayerCard>();
    [SerializeField] private Transform parentForSeats;
    public GameObject playerCardPrefab;

    public List<TableCard> tableCards = new List<TableCard>();
    [SerializeField] private Transform partentForCards;
    public GameObject tableCardPrefab;

    int playersToConnect = Settings.PlayerCount;

    [HideInInspector] public Deck deck;
    public Card[] flippedCommunityCards;

    private int lastDealerIndex = 0;
    private int lastSmallBlindIndex = 1;
    private int lastBigBlindIndex = 2;

    [HideInInspector] public int moneyInPot;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Loader.Instance.StartLoading();
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
        newPlayer.turnInfo = playerTurnInfo;
        newPlayer.indexInConnectionsArray = indexOfConnection;

        playerSeats.Add(newPlayer);

        if (playersToConnect == 0)
        {
            AssignRolesAndShuffleTheOrderOfPlayers();
            MatchManager.Instance.SendGameInfoToPlayers();
            TurnManager.Instance.StartFirstTurn();
            Loader.Instance.StopLoading();
        }
    }

    public int PlayersInGameCount => playerSeats
        .FindAll(player => player.IsStillInGame).Count;

    private void AssignRolesAndShuffleTheOrderOfPlayers()
    {
        playerSeats = playerSeats.OrderBy(x => Random.Range(0, 100)).ToList();

        playerSeats[0].isDealer = true;
        playerSeats[1].isSmallBlind = true;
        playerSeats[2].isBigBlind = true;

        for (int i = 0; i < playerSeats.Count; i++)
        {
            TableGUI.Instance.DisplayPlayer(playerSeats[i], i);
        }
    }

    public void DealCardsToPlayers()
    {
        for (int i = 0; i < playerSeats.Count; i++)
        {
            Card[] cards = TexasHoldEm.DealCardsToPlayer(deck);
            DealCardsMessage dealCardsMessage = new DealCardsMessage();
            playerSeats[i].turnInfo.cards = cards;
            dealCardsMessage.cards = cards;
            int indexInConnections = playerSeats[i].indexInConnectionsArray;
            ConnectionManager.Instance.SendMessageToConnection(ConnectionManager.Instance.Connections[indexInConnections], dealCardsMessage);
        }
    }

    public void DealCardsToTable()
    {
        for (int i = 0; i < 5; i++)
        {
            tableCards.Add(Instantiate(tableCardPrefab, partentForCards).GetComponent<TableCard>());
            tableCards[i].card = deck.Draw();
            TableGUI.Instance.DisplayCard(tableCards[i], i);
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
        CommunityCardsChanged communityCardsChanged = new CommunityCardsChanged() { communityCards = flippedCommunityCards };
        ConnectionManager.Instance.SendMessageToAllConnections(communityCardsChanged);
    }

    public void DealTurn()
    {
        flippedCommunityCards = new Card[4];
        tableCards[3].Flip();
        flippedCommunityCards[0] = tableCards[0].card;
        flippedCommunityCards[1] = tableCards[1].card;
        flippedCommunityCards[2] = tableCards[2].card;
        flippedCommunityCards[3] = tableCards[3].card;
        CommunityCardsChanged communityCardsChanged = new CommunityCardsChanged() { communityCards = flippedCommunityCards };
        ConnectionManager.Instance.SendMessageToAllConnections(communityCardsChanged);
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
        CommunityCardsChanged communityCardsChanged = new CommunityCardsChanged() { communityCards = flippedCommunityCards };
        ConnectionManager.Instance.SendMessageToAllConnections(communityCardsChanged);
    }

    public void PlayerTurnDone(TurnDoneMessage turnDoneMessage)
    {
        playerSeats.Find(p => p.turnInfo.player.Equals(turnDoneMessage.player)).RefreshMoney(turnDoneMessage.newMoney);
        playerSeats.Find(p => p.turnInfo.player.Equals(turnDoneMessage.player)).TurnDone();
        TurnManager.Instance.HandleTurnDone(turnDoneMessage);
    }

    public void GivePotToWinners(PlayerHandInfo[] winners)
    {
        foreach (PlayerHandInfo winner in winners)
        {
            TablePlayerCard winnerCard = playerSeats.Find(p => p.turnInfo.player.Equals(winner.Player));
            winnerCard.turnInfo.money += moneyInPot / winners.Length;
            winnerCard.turnInfo.wentAllIn = false;
            winnerCard.RefreshMoney(playerSeats.Find(p => p.turnInfo.player.Equals(winner.Player)).turnInfo.money);
        }
        moneyInPot = 0;
    }

    public void StartNewGame()
    {
        ClearTable();
        ReshufleDeck();
        ResetAndRotatePlayers();
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

    private void ReshufleDeck()
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

        if (!player.IsStillInGame)
        {
            player = TurnManager.Instance.GetNextPlayerStillInGame(index);
            index = playerSeats.IndexOf(player);
        }

        return index;
    }

    public void PlayerDisconnected(Player player)
    {
        TablePlayerCard playerCard = playerSeats.Find(p => p.turnInfo.player.Equals(player));
        playerCard.Disconnected();
        playerCard.turnInfo.money = 0;
    }

    public void SendGameOverMessages()
    {
        List<TablePlayerCard> playersThatAreOut = playerSeats.Select(p => p).Where(p => !p.IsStillInGame).ToList();
    }
}
