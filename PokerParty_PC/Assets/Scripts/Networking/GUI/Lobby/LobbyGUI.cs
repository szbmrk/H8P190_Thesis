using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PokerParty_SharedDLL;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyGUI : MonoBehaviour
{
    public static LobbyGUI Instance;
    public TextMeshProUGUI joinCodeText;

    [SerializeField] private Button deleteLobbyBtn;
    [SerializeField] private Button startGameBtn;
    [SerializeField] private GameObject minimumPlayersText;

    [SerializeField] private TextMeshProUGUI playerCount;
    [SerializeField] private GameObject LobbyPanel;
    [SerializeField] private LobbyPlayerCard playerCardPrefab;
    [SerializeField] private Transform parentForPlayerCards;

    private int numOfPlayers = 0;
    private int yOffset = -110;

    private void Start()
    {
        if (Instance == null)
            Instance = this;

        deleteLobbyBtn.onClick.AddListener(() => StartCoroutine(DeleteLobby()));
        startGameBtn.onClick.AddListener(StartGame);
    }
    
    public void StartGame()
    {
        Settings.StartingMoney = 5000;
        Settings.PlayerCount = numOfPlayers;
        ConnectionManager.Instance.SendMessageToAllConnections(new GameStartedMessage());
        SceneManager.LoadScene("Game");
    }

    public IEnumerator DeleteLobby()
    {
        Loader.Instance.StartLoading();
        joinCodeText.text = string.Empty;
        ClearPlayers();
        ChatGUI.Instance.ClearChat();
        yield return LobbyManager.Instance.DeleteLobby();

        SceneManager.LoadScene("MainMenu");
    }

    public LobbyPlayerCard DisplayNewPlayer(Player player)
    {
        Transform playerCardTransform = null;

        if (numOfPlayers >= parentForPlayerCards.childCount)
        {
            playerCardTransform = Instantiate(playerCardPrefab, parentForPlayerCards).transform;
        }
        else
        {
            for (int i = 0; i < parentForPlayerCards.childCount; i++)
            {
                LobbyPlayerCard playerCardComponent = parentForPlayerCards.GetChild(i).GetComponent<LobbyPlayerCard>();
                if (playerCardComponent.isPlayerAssigned == false)
                {
                    playerCardTransform = parentForPlayerCards.GetChild(i);
                }
            }
        }

        LobbyPlayerCard playerCard = playerCardTransform.GetComponent<LobbyPlayerCard>();

        playerCard.assignedPlayer = player;
        playerCard.isPlayerAssigned = true;
        playerCardTransform.GetComponent<LobbyPlayerCard>().RefreshData();

        playerCard.gameObject.SetActive(true);

        numOfPlayers++;

        RefreshPlayerCount();
        RepositionCards();

        return playerCard;
    }

    public void RemovePlayerFromDisplay(Player player)
    {

        if (numOfPlayers == 0) return;

        LobbyPlayerCard playerCard = LobbyManager.Instance.GetPlayerCardForPlayer(player);

        if (playerCard == null) return;

        playerCard.ResetData();
        playerCard.gameObject.SetActive(false);

        numOfPlayers--;
        RefreshPlayerCount();
        RepositionCards();
    }

    public void ClearPlayers()
    {
        if (numOfPlayers == 0) return;

        for (int i = 0; i < parentForPlayerCards.childCount; i++)
        {
            Destroy(parentForPlayerCards.GetChild(i).gameObject);
        }

        numOfPlayers = 0;
        RefreshPlayerCount();
    }

    private void RepositionCards()
    {
        for (int i = 0; i < parentForPlayerCards.childCount; i++)
        {
            float yPos = i * yOffset;
            parentForPlayerCards.GetChild(i).GetComponent<RectTransform>().localPosition = new Vector3(0, yPos, 0);
        }
    }

    private void RefreshPlayerCount()
    {
        if (numOfPlayers >= 4 && LobbyManager.Instance.AreAllPlayersReady())
        {
            startGameBtn.interactable = true;
            minimumPlayersText.SetActive(false);
        }
        else
        {
            startGameBtn.interactable = false;
            minimumPlayersText.SetActive(true);
        }

        playerCount.text = numOfPlayers.ToString() + "/8";
    }

    public void ShowPanel()
    {
        LobbyPanel.SetActive(true);
    }
}
