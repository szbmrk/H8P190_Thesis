using System.Collections.Generic;
using NUnit.Framework;
using PokerParty_SharedDLL;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TablePlayerCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI lastActionText;

    [SerializeField] private GameObject dealerIcon;
    [SerializeField] private GameObject smallBlindIcon;
    [SerializeField] private GameObject bigBlindIcon;
    [SerializeField] private GameObject turnIcon;
    [SerializeField] private GameObject outOfTurn;
    [SerializeField] private GameObject foldedTxt;
    [SerializeField] private GameObject allInTxt;

    [SerializeField] private GameObject handGameObject;
    [SerializeField] private Image card1;
    [SerializeField] private Image card2;
    [SerializeField] private TextMeshProUGUI handTypeText;

    [HideInInspector] public PlayerTurnInfo TurnInfo;
    [HideInInspector] public int indexInConnectionsArray;

    [HideInInspector] public bool isDealer;
    [HideInInspector] public bool isSmallBlind;
    [HideInInspector] public bool isBigBlind;

    [HideInInspector] public bool isTurn;

    public bool isStillInGame => TurnInfo.Money > 0 && !TurnInfo.Folded && !TurnInfo.WentAllIn;

    public void LoadData()
    {
        playerNameText.color = PlayerColorManager.GetColor(TurnInfo.Player.PlayerName);
        playerNameText.text = TurnInfo.Player.PlayerName;

        SetRoleIcons();
        RefreshMoney(Settings.startingMoney);
        RefreshTurnIcon();
    }

    public void Reset()
    {
        isDealer = false;
        isSmallBlind = false;
        isBigBlind = false;
        isTurn = false;
        TurnInfo.MoneyPutInPot = 0;
        TurnInfo.Cards = new Card[2];
        TurnInfo.Folded = false;
        TurnInfo.WentAllIn = false;
        lastActionText.text = string.Empty;
        outOfTurn.SetActive(false);
        foldedTxt.SetActive(false);
        allInTxt.SetActive(false);

        RefreshTurnIcon();
    }
    
    public void Fold()
    {
        outOfTurn.SetActive(true);
        TurnInfo.Folded = true;
        foldedTxt.SetActive(true);
        TurnDone();
    }
    
    public void AllIn()
    {
        outOfTurn.SetActive(true);
        TurnInfo.WentAllIn = true;
        allInTxt.SetActive(true);
    }

    public void OutOfGame()
    {
        Reset();
        gameObject.SetActive(false);
    }
    
    public void Disconnected()
    {
        OutOfGame();
        TurnDoneMessage turnDoneMessage = new TurnDoneMessage
        {
            Action = PossibleAction.Fold
        };
        
        TurnManager.instance.HandleTurnDone(turnDoneMessage);
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
        playerNameText.text = isTurn ? $"<b>{TurnInfo.Player.PlayerName}</b>" : TurnInfo.Player.PlayerName;
    }

    public void RefreshMoney(int money)
    {
        moneyText.text = $"{money}$";
    }
    
    public void SetLastActionText(string text)
    {
        lastActionText.text = text;
    }

    public void StartTurn()
    {
        isTurn = true;
        GameManager.instance.SetWaitingFor(TurnInfo.Player.PlayerName);
        RefreshTurnIcon();
    }

    public void TurnDone()
    {
        isTurn = false;
        RefreshTurnIcon();
    }

    public void ShowHand()
    {
        card1.sprite = CardHelper.GetSpriteByFileName(TurnInfo.Cards[0].GetFileNameForSprite());
        card2.sprite = CardHelper.GetSpriteByFileName(TurnInfo.Cards[1].GetFileNameForSprite());

        List<Card> allCards = new List<Card>();
        allCards.Add(TurnInfo.Cards[0]);
        allCards.Add(TurnInfo.Cards[1]);
        foreach (TableCard tableCard in TableManager.instance.tableCards)
        {
            Card card = new Card(tableCard.card.Value, tableCard.card.Suit);
            allCards.Add(card);
        }

        string debugString = $"{TurnInfo.Player.PlayerName}:";

        debugString += "card:";
        foreach (Card card in allCards)
        {
            debugString += $" {card}";
        }
        
        Card[][] possibleHands = TexasHoldEm.GetAllPossibleHands(allCards.ToArray());
        
        debugString += $"\n{possibleHands.Length} possible hands";
        
        foreach (Card[] possibleHand in possibleHands)
        {
            debugString += "\n";
            foreach (Card card in possibleHand)
            {
                debugString += $" {card}";
            }
        }
        
        debugString += $"\nHand eval of possiblehands:";
        foreach (Card[] possibleHand in possibleHands)
        {
            HandType handType = TexasHoldEm.EvaluateHand(possibleHand);
            debugString += $" {handType}";
        }
        
        Card[] bestHand = TexasHoldEm.GetBestHandOfPlayer(possibleHands);
        HandType bestHandType = TexasHoldEm.EvaluateHand(bestHand);
        
        handTypeText.text = bestHandType.ToString();
        
        
        debugString += "\nBest hand:";
        foreach (Card card in bestHand)
        {
            debugString += $" {card}";
        }
        
        debugString += "\nBest hand type:";
        debugString += bestHandType;
        
        Debug.Log(debugString);
        
        handGameObject.SetActive(true);
    }
    
    public void ResetHand()
    {
        handGameObject.SetActive(false);
    }

    public override bool Equals(object other)
    {
        if (other == null || !(other is TablePlayerCard))
            return false;

        TablePlayerCard otherPlayer = other as TablePlayerCard;

        return otherPlayer.TurnInfo.Player.Equals(TurnInfo.Player);
    }

    public override int GetHashCode()
    {
        return TurnInfo.Player.GetHashCode();
    }
}
