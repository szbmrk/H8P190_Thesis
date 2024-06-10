using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using PokerParty_SharedDLL;
using UnityEngine.UI;

public class LobbyGUI : MonoBehaviour
{
    public static LobbyGUI Instance;

    public TextMeshProUGUI joinCodeText;

    [SerializeField] private Button sendTestMessageBtn;
    [SerializeField] private Button disconnectAllPlayersBtn;
    [SerializeField] private Button deleteLobbyBtn;

    [SerializeField] private TextMeshProUGUI playerCount;
    [SerializeField] private GameObject LobbyPanel;
    [SerializeField] private PlayerCard playerCardPrefab;
    [SerializeField] private Transform parentForPlayerCards;

    private int numOfPlayers = 0;
    private int yOffset = -110;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        sendTestMessageBtn.onClick.AddListener(() => RelayManager.Instance.SendMessageToPlayers("hello from host"));
        disconnectAllPlayersBtn.onClick.AddListener(DisconnectAllPlayers);
        deleteLobbyBtn.onClick.AddListener(DeleteLobby);
    }

    public void DisplayNewPlayer(Player player)
    {
        if (numOfPlayers >= parentForPlayerCards.childCount)
        {
            Instantiate(playerCardPrefab, parentForPlayerCards);
        }

        Transform playerCard = null;
        for (int i = 0; i < parentForPlayerCards.childCount; i++)
        {
            PlayerCard playerCardComponent = parentForPlayerCards.GetChild(i).GetComponent<PlayerCard>();
            if (playerCardComponent.isPlayerAssigned == false) 
            {
                playerCard = parentForPlayerCards.GetChild(i);
            }
        }

        playerCard.GetComponent<PlayerCard>().assignedPlayer = player;
        playerCard.GetComponent<PlayerCard>().isPlayerAssigned = true;
        playerCard.GetComponent<PlayerCard>().RefreshData();

        float yPos = numOfPlayers * yOffset;
        playerCard.GetComponent<RectTransform>().localPosition = new Vector3(0, yPos, 0);

        playerCard.gameObject.SetActive(true);

        numOfPlayers++;
        RefreshPlayerCountText();
    }

    public void RemovePlayerFromDisplay(Player player)
    {

        if (numOfPlayers == 0) return;

        for (int i = 0; i < parentForPlayerCards.childCount; i++)
        {
            PlayerCard playerCard = parentForPlayerCards.GetChild(i).GetComponent<PlayerCard>();
            if (playerCard.assignedPlayer.Equals(player))
            {
                playerCard.assignedPlayer = null;
                playerCard.isPlayerAssigned = false;
                playerCard.gameObject.SetActive(false);
            }
        }

        numOfPlayers--;
        RefreshPlayerCountText();
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

    private void DisconnectAllPlayers()
    {
        RelayManager.Instance.DisconnectAllPlayers();
        ClearDisplay();
    }

    private void RefreshPlayerCountText()
    {
        playerCount.text = numOfPlayers.ToString() + "/8";
    }

    private void DeleteLobby()
    {
        RelayManager.Instance.DeleteLobby();
        joinCodeText.text = "";
        LobbyPanel.SetActive(false);
    }

    public void ShowPanel()
    {
        LobbyPanel.SetActive(true);
    }
}
