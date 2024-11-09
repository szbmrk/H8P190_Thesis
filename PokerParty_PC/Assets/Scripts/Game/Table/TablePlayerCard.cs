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
    [SerializeField] private TextMeshProUGUI MoneyPutInText;

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

    [HideInInspector] public bool IsStillInGame
    {
        get
        {
            return turnInfo.money > 0 && !turnInfo.folded && !turnInfo.wentAllIn && !turnInfo.isOutOfGame;
        }
    }

    public void LoadData()
    {
        playerNameText.color = PlayerColorManager.GetColor(turnInfo.player.playerName);
        playerNameText.text = turnInfo.player.playerName;
        ELOText.text = $"ELO: {turnInfo.player.ELO}";
        LevelText.text = $"Level: {turnInfo.player.level}";

        SetRoleIcons();
        RefreshMoney(Settings.StartingMoney);
        RefreshTurnIcon();
    }

    public void Reset()
    {
        isDealer = false;
        isSmallBlind = false;
        isBigBlind = false;
        isTurn = false;
        turnInfo.moneyPutInPot = 0;
        turnInfo.cards = new Card[2];
        turnInfo.folded = false;
        MoneyPutInText.text = "Put in: 0 $";
        outOfTurn.SetActive(false);

        if (turnInfo.money <= 0)
        {
            turnInfo.isOutOfGame = true;
            OutOfTurn();
        }

        RefreshTurnIcon();
    }

    public void SetRoleIcons()
    {
        dealerIcon.SetActive(isDealer);
        smallBlindIcon.SetActive(isSmallBlind);
        bigBlindIcon.SetActive(isBigBlind);
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

    public void RefreshMoneyPutIn(int money)
    {
        MoneyPutInText.text = $"Put in: {money} $";
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
