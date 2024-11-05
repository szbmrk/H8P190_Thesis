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
    [HideInInspector] public int moneyInPot = 0;

    // 1% of starting money
    [HideInInspector] public int smallBlindBet;
    // 2% of starting money
    [HideInInspector] public int bigBlindBet;

    public Card[] flippedCommunityCards;
    private int turnCount = 0;

    private void Awake()
    {
        Instance = this;
        smallBlindBet = (int)(Settings.StartingMoney * 0.01);
        bigBlindBet = (int)(Settings.StartingMoney * 0.02);
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
            gameInfoMessage.startingMoney = Settings.StartingMoney;
            gameInfoMessage.smallBlindAmount = smallBlindBet;
            gameInfoMessage.bigBlindAmount = bigBlindBet;
            gameInfoMessage.isDealer = playerSeats[i].isDealer;
            gameInfoMessage.isSmallBlind = playerSeats[i].isSmallBlind;
            gameInfoMessage.isBigBlind = playerSeats[i].isBigBlind;

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

    public IEnumerator ShowDown()
    {
        PlayerHandInfo[] winners = EvaluationHelper.DetermineWinners(playerSeats.ToArray());
        GivePotToWinners(winners);
        moneyInPot = 0;
        TableGUI.Instance.RefreshMoneyInPotText(moneyInPot);

        if (winners.Length == 1)
            yield return TableGUI.Instance.showTurnWinner(winners[0].Player.playerName);
        else
        {
            string winnerText = "";
            for (int i = 0; i < winners.Length; i++)
            {
                if (i == winners.Length - 1)
                    winnerText += winners[i].Player.playerName;
                else
                    winnerText += winners[i].Player.playerName + ", ";
            }
            yield return TableGUI.Instance.showTurnWinner(winnerText);
        }

        turnCount++;
    }

    private void GivePotToWinners(PlayerHandInfo[] winners)
    {
        foreach (PlayerHandInfo winner in winners)
        {
            playerSeats.Find(p => p.turnInfo.player.Equals(winner.Player)).turnInfo.money += moneyInPot / winners.Length;
            playerSeats.Find(p => p.turnInfo.player.Equals(winner.Player)).RefreshMoney(playerSeats.Find(p => p.turnInfo.player.Equals(winner.Player)).turnInfo.money);
        }
    }

    public void ResetAndRotatePlayers()
    {
        for (int i = 0; i < playerSeats.Count; i++)
        {
            playerSeats[i].Reset();
        }

        if (TurnManager.Instance.IsPlayerStillInGame(playerSeats[0 + turnCount]))
        {
            playerSeats[0 + turnCount].isDealer = true;
        }
        else
        {
            TurnManager.Instance.GetNextPlayerStillInGame(0 + turnCount).isDealer = true;
        }

        if (TurnManager.Instance.IsPlayerStillInGame(playerSeats[1 + turnCount]))
        {
            playerSeats[1 + turnCount].isSmallBlind = true;
        }
        else
        {
            TurnManager.Instance.GetNextPlayerStillInGame(1 + turnCount).isSmallBlind = true;
        }

        if (TurnManager.Instance.IsPlayerStillInGame(playerSeats[2 + turnCount]))
        {
            playerSeats[2 + turnCount].isBigBlind = true;
        }
        else
        {
            TurnManager.Instance.GetNextPlayerStillInGame(2 + turnCount).isBigBlind = true;
        }

        foreach (TablePlayerCard player in playerSeats)
        {
            player.SetRoleIcons();
        }
    }
}
