using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PokerParty_SharedDLL;
using System.Linq;

public class TableManager : MonoBehaviour
{
    public static TableManager Instance;
    
    private List<TablePlayerCard> playerSeats = new List<TablePlayerCard>();
    [SerializeField] private Transform parentForSeats;
    
    public GameObject playerCardPrefab;

    int playersToConnect = Settings.PlayerCount;

    private Deck deck;
    [HideInInspector] public int turnCount = 0;
    [HideInInspector] public int moneyInPot = 0;

    // 1% of starting money
    [HideInInspector] public int smallBlindBet = (int)(Settings.StartingMoney * 0.01);
    // 2% of starting money
    [HideInInspector] public int bigBlindBet = (int)(Settings.StartingMoney * 0.02);

    private void Awake()
    {
        if (Instance == null)
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
        Debug.Log("Deck created and shuffled");
        Debug.Log("Deck count: " + deck.cards.Count);
    }

    public void PlayerLoaded(Player player, int indexOfConnection)
    {
        playersToConnect--;
        TablePlayerCard newPlayer = Instantiate(playerCardPrefab, parentForSeats).GetComponent<TablePlayerCard>();
        newPlayer.assignedPlayer = player;
        newPlayer.indexInConnectionsArray = indexOfConnection;
        newPlayer.gameObject.SetActive(false);
        playerSeats.Add(newPlayer);

        if (playersToConnect == 0)
        {
            AssignRolesAndShuffleTheOrderOfPlayers();
            Loader.Instance.StopLoading();
            ConnectionManager.Instance.SendMessageToAllConnections(new EveryoneLoadedMessage());
            StartFirstTurn();
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

    public void StartFirstTurn()
    {
        turnCount++;
        playerSeats[1].StartTurn();
    }
}
