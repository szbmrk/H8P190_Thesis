using PokerParty_SharedDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TablePlayerCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI ELOText;
    [SerializeField] private TextMeshProUGUI LevelText;
    [SerializeField] private TextMeshProUGUI MoneyText;

    [SerializeField] private GameObject dealerIcon;
    [SerializeField] private GameObject smallBlindIcon;
    [SerializeField] private GameObject bigBlindIcon;
    [SerializeField] private GameObject turnIcon;

    [HideInInspector] public Player assignedPlayer;
    [HideInInspector] public int indexInConnectionsArray = 0;

    [HideInInspector] public int money;

    [HideInInspector] public bool isDealer;
    [HideInInspector] public bool isSmallBlind;
    [HideInInspector] public bool isBigBlind;

    [HideInInspector] public bool isTurn;

    private void Start()
    {
        money = Settings.StartingMoney;
    }

    public void LoadData()
    {
        playerNameText.color = PlayerColorManager.GetColor(assignedPlayer.playerName);
        playerNameText.text = assignedPlayer.playerName;
        ELOText.text = $"ELO: {assignedPlayer.ELO}";
        LevelText.text = $"Level: {assignedPlayer.level}";

        dealerIcon.SetActive(isDealer);
        smallBlindIcon.SetActive(isSmallBlind);
        bigBlindIcon.SetActive(isBigBlind);

        RefreshData();
    }

    public void RefreshData()
    {
        MoneyText.text = $"{money} $";
        turnIcon.SetActive(isTurn);
        if (isTurn)
            playerNameText.text = $"<b>{assignedPlayer.playerName}</b>";
        else
            playerNameText.text = assignedPlayer.playerName;
    }

    public void StartTurn()
    {
        isTurn = true;
        GameManager.Instance.SetWaitingFor(assignedPlayer.playerName);
        RefreshData();
    }
}
