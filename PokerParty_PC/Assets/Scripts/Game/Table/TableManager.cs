using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PokerParty_SharedDLL;
using System.Linq;

public class TableManager : MonoBehaviour
{
    public static TableManager Instance;
    private List<TablePlayerCard> playerSeats = new List<TablePlayerCard>();
    private Deck deck;

    int playersToConnect = Settings.PlayerCount;

    public List<Transform> seatPositions = new List<Transform>();

    public GameObject playerCardPrefab;

    [HideInInspector] public int turnCount = 0;

    [HideInInspector] public int moneyInPot = 0;

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
        TablePlayerCard newPlayer = Instantiate(playerCardPrefab).GetComponent<TablePlayerCard>();
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
        playerSeats.ForEach(x => x.gameObject.SetActive(true));

        playerSeats[0].isDealer = true;
        playerSeats[1].isSmallBlind = true;
        playerSeats[2].isBigBlind = true;

        for (int i = 0; i < playerSeats.Count; i++)
        {
            playerSeats[i].transform.position = seatPositions[i].position;
            playerSeats[i].LoadData();
        }
    }

    public void StartFirstTurn()
    {
        turnCount++;
        playerSeats[1].StartTurn();
    }
}
