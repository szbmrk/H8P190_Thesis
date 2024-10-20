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

    int playersToConnect = Settings.PlayerCount;

    [HideInInspector] public Deck deck;
    [HideInInspector] public int moneyInPot = 0;

    // 1% of starting money
    [HideInInspector] public int smallBlindBet = (int)(Settings.StartingMoney * 0.01);
    // 2% of starting money
    [HideInInspector] public int bigBlindBet = (int)(Settings.StartingMoney * 0.02);

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
            SendGameInfoToPlayers();
            TurnManager.Instance.StartFirstTurn();
            Loader.Instance.StopLoading();
        }
    }

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

    public void SendGameInfoToPlayers()
    {
        for (int i = 0; i < playerSeats.Count; i++)
        {
            GameInfoMessage gameInfoMessage = new GameInfoMessage();
            gameInfoMessage.StartingMoney = Settings.StartingMoney;
            gameInfoMessage.SmallBlindAmount = smallBlindBet;
            gameInfoMessage.BigBlindAmount = bigBlindBet;
            gameInfoMessage.IsDealer = playerSeats[i].isDealer;
            gameInfoMessage.IsSmallBlind = playerSeats[i].isSmallBlind;
            gameInfoMessage.IsBigBlind = playerSeats[i].isBigBlind;

            int indexInConnections = playerSeats[i].indexInConnectionsArray;
            ConnectionManager.Instance.SendMessageToConnection(ConnectionManager.Instance.Connections[indexInConnections], gameInfoMessage);
        }
    }

    public void DealCardsToPlayers()
    {
        for (int i = 0; i < playerSeats.Count; i++)
        {
            Card[] cards = TexasHoldEm.DealCardsToPlayer(deck);
            DealCardsMessage dealCardsMessage = new DealCardsMessage();
            dealCardsMessage.Cards = cards;
            int indexInConnections = playerSeats[i].indexInConnectionsArray;
            ConnectionManager.Instance.SendMessageToConnection(ConnectionManager.Instance.Connections[indexInConnections], dealCardsMessage);
        }
    }

    public void DealFlop()
    {

    }

    public void DealTurn()
    {

    }

    public void DealRiver()
    {

    }

    public void PlayerTurnDone(TurnDoneMessage turnDoneMessage)
    {
        playerSeats.Find(p => p.turnInfo.player.Equals(turnDoneMessage.player)).RefreshMoney(turnDoneMessage.NewMoney);
        playerSeats.Find(p => p.turnInfo.player.Equals(turnDoneMessage.player)).TurnDone();
        TurnManager.Instance.HandleTurnDone(turnDoneMessage);
    }
}
