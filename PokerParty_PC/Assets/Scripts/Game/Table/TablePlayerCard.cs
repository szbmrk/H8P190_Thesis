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

    [HideInInspector] public bool isStillInGame => turnInfo.money > 0 && !turnInfo.folded && !turnInfo.wentAllIn;

    public void LoadData()
    {
        playerNameText.color = PlayerColorManager.GetColor(turnInfo.player.PlayerName);
        playerNameText.text = turnInfo.player.PlayerName;

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
        turnInfo.wentAllIn = false;
        MoneyPutInText.text = "Put in: 0 $";
        outOfTurn.SetActive(false);

        if (turnInfo.money <= 0 && gameObject.activeInHierarchy)
        {
            OutOfTurn();
            GameManager.Instance.SendGameOverMessageToPlayer(indexInConnectionsArray);
        }

        RefreshTurnIcon();
    }

    public void OutOfGame()
    {
        Reset();
        gameObject.SetActive(false);
    }
    
    public void Disconnected()
    {
        OutOfGame();
        TurnDoneMessage turnDoneMessage = new TurnDoneMessage();
        turnDoneMessage.action = PossibleAction.FOLD;
        TurnManager.Instance.HandleTurnDone(turnDoneMessage);
    }

    public void SetRoleIcons()
    {
        dealerIcon.SetActive(isDealer);
        smallBlindIcon.SetActive(isSmallBlind);
        bigBlindIcon.SetActive(isBigBlind);
    }

    private void RefreshTurnIcon()
    {
        turnIcon.SetActive(isTurn);
        playerNameText.text = isTurn ? $"<b>{turnInfo.player.PlayerName}</b>" : turnInfo.player.PlayerName;
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
        GameManager.Instance.SetWaitingFor(turnInfo.player.PlayerName);
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
