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
    [SerializeField] private GameObject outOfTurn;

    [HideInInspector] public PlayerTurnInfo turnInfo;
    [HideInInspector] public int indexInConnectionsArray;

    [HideInInspector] public bool isDealer;
    [HideInInspector] public bool isSmallBlind;
    [HideInInspector] public bool isBigBlind;

    [HideInInspector] public bool isTurn;

    public void LoadData()
    {
        playerNameText.color = PlayerColorManager.GetColor(turnInfo.player.playerName);
        playerNameText.text = turnInfo.player.playerName;
        ELOText.text = $"ELO: {turnInfo.player.ELO}";
        LevelText.text = $"Level: {turnInfo.player.level}";

        dealerIcon.SetActive(isDealer);
        smallBlindIcon.SetActive(isSmallBlind);
        bigBlindIcon.SetActive(isBigBlind);
        RefreshMoney(Settings.StartingMoney);
        RefreshTurnIcon();
    }

    public void RefreshTurnIcon()
    {
        turnIcon.SetActive(isTurn);
        if (isTurn)
            playerNameText.text = $"<b>{turnInfo.player.playerName}</b>";
        else
            playerNameText.text = turnInfo.player.playerName;
    }

    public void RefreshMoney(int money)
    {
        MoneyText.text = $"{money} $";
    }

    public void StartTurn()
    {
        isTurn = true;
        GameManager.Instance.SetWaitingFor(turnInfo.player.playerName);
        RefreshTurnIcon();
    }

    public void TurnDone()
    {
        isTurn = false;
        RefreshTurnIcon();
    }

    public void OutOfTurn()
    {
        outOfTurn.SetActive(true);
    }

    public override bool Equals(object other)
    {
        if (other == null || !(other is TablePlayerCard))
            return false;

        TablePlayerCard otherPlayer = other as TablePlayerCard;

        return otherPlayer.turnInfo.player.Equals(turnInfo.player);
    }

    public override int GetHashCode()
    {
        return turnInfo.player.GetHashCode();
    }
}
