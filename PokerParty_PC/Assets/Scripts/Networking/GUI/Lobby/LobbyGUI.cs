using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PokerParty_SharedDLL;
using UnityEngine.UI;

public class LobbyGUI : MonoBehaviour
{
    public static LobbyGUI Instance;
    public TextMeshProUGUI joinCodeText;

    [SerializeField] private Button disconnectAllPlayersBtn;
    [SerializeField] private Button deleteLobbyBtn;
    [SerializeField] private Button startGameBtn;

    [SerializeField] private TextMeshProUGUI playerCount;
    [SerializeField] private GameObject LobbyPanel;
    [SerializeField] private PlayerCard playerCardPrefab;
    [SerializeField] private Transform parentForPlayerCards;

    private int numOfPlayers = 0;
    private int yOffset = -110;

    private void Start()
    {
        if (Instance == null)
            Instance = this;

        disconnectAllPlayersBtn.onClick.AddListener(LobbyManager.Instance.DisconnectAllPlayers);
        deleteLobbyBtn.onClick.AddListener(LobbyManager.Instance.DeleteLobby);
    }

    private void Update()
    {
        if (LobbyManager.Instance.joinedPlayers.Count >= 3 && LobbyManager.Instance.AreAllPlayersReady())
        {
            startGameBtn.interactable = true;
        }
        else
        {
            startGameBtn.interactable = false;
        }
    }

    public PlayerCard DisplayNewPlayer(Player player)
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
                PlayerCard playerCardComponent = parentForPlayerCards.GetChild(i).GetComponent<PlayerCard>();
                if (playerCardComponent.isPlayerAssigned == false)
                {
                    playerCardTransform = parentForPlayerCards.GetChild(i);
                }
            }
        }

        PlayerCard playerCard = playerCardTransform.GetComponent<PlayerCard>();

        playerCard.assignedPlayer = player;
        playerCard.isPlayerAssigned = true;
        playerCardTransform.GetComponent<PlayerCard>().RefreshData();

        playerCard.gameObject.SetActive(true);

        numOfPlayers++;

        RefreshPlayerCountText();
        RepositionCards();

        return playerCard;
    }

    public void RemovePlayerFromDisplay(Player player)
    {

        if (numOfPlayers == 0) return;

        PlayerCard playerCard = LobbyManager.Instance.GetPlayerCardForPlayer(player);

        if (playerCard == null) return;

        playerCard.assignedPlayer = null;
        playerCard.isPlayerAssigned = false;
        playerCard.SetReady(false);
        playerCard.gameObject.SetActive(false);
        PlayerColorManager.RemoveColorFromPlayer(player.username);

        numOfPlayers--;
        RefreshPlayerCountText();
        RepositionCards();
    }

    public void ClearDisplay()
    {
        if (numOfPlayers == 0) return;

        for (int i = 0; i < parentForPlayerCards.childCount; i++)
        {
            Destroy(parentForPlayerCards.GetChild(i).gameObject);
        }

        numOfPlayers = 0;
        RefreshPlayerCountText();
    }

    private void RepositionCards()
    {
        for (int i = 0; i < parentForPlayerCards.childCount; i++)
        {
            float yPos = i * yOffset;
            parentForPlayerCards.GetChild(i).GetComponent<RectTransform>().localPosition = new Vector3(0, yPos, 0);
        }
    }

    private void RefreshPlayerCountText()
    {
        playerCount.text = numOfPlayers.ToString() + "/8";
    }

    public void ShowPanel()
    {
        LobbyPanel.SetActive(true);
    }

    public void HidePanel()
    {
        LobbyPanel.SetActive(false);
    }
}
