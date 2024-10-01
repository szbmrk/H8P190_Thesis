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
        Transform playerCardTransform = Instantiate(playerCardPrefab, parentForPlayerCards).transform;
        LobbyPlayerCard playerCard = playerCardTransform.GetComponent<LobbyPlayerCard>();
        playerCard.assignedPlayer = player;
        playerCardTransform.GetComponent<LobbyPlayerCard>().RefreshData();

        playerCard.gameObject.SetActive(true);

        LayoutRebuilder.ForceRebuildLayoutImmediate(parentForPlayerCards.GetComponent<RectTransform>());

        numOfPlayers++;
        RefreshPlayerCount();
        return playerCard;
    }

    public void RemovePlayerFromDisplay(Player player)
    {
        if (numOfPlayers == 0) return;

        LobbyPlayerCard playerCard = LobbyManager.Instance.GetPlayerCardForPlayer(player);

        if (playerCard == null) return;

        playerCard.ResetData();
        Destroy(playerCard.gameObject);

        numOfPlayers--;
        RefreshPlayerCount();
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

    public void RefreshPlayerCount()
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
