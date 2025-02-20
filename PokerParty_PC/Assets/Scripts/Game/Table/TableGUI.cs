using System.Collections;
using System.Collections.Generic;
using PokerParty_SharedDLL;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TableGUI : MonoBehaviour
{
    public static TableGUI instance;
    public List<Transform> seatPositions = new List<Transform>();
    public List<Transform> cardPositions = new List<Transform>();
    public Transform drawingDeckPosition;

    [SerializeField] private TextMeshProUGUI moneyInPotText;
    [SerializeField] private TextMeshProUGUI turnWinnerText;
    [SerializeField] private TextMeshProUGUI winnerHandText;
    
    public float cardSpeed = 1f;
    public LeanTweenType cardEase = LeanTweenType.easeInOutQuint;

    private void Awake()
    {
        instance = this;
    }

    public void DisplayPlayer(TablePlayerCard card, int i)
    {
        card.transform.position = seatPositions[i].position;
        card.gameObject.SetActive(true);
        card.LoadData();
    }

    public IEnumerator DisplayCard(TableCard card, int i)
    {
        Transform startPos = drawingDeckPosition;
        Transform goalPos = cardPositions[i];
        
        card.transform.position = startPos.position;
        card.GetComponent<SpriteRenderer>().sortingOrder = 3;
        card.gameObject.SetActive(true);
        
        float distance = Vector3.Distance(startPos.position, goalPos.position);
        float duration = distance / cardSpeed;

        LeanTween.move(card.gameObject, goalPos.position, duration).setEase(cardEase);

        yield return new WaitForSeconds(duration);
    }

    public void RefreshMoneyInPotText(int potMoney)
    {
        moneyInPotText.text = $"Pot: {potMoney}$";
    }

    public IEnumerator ShowTurnWinner(string winnerName, HandType handType, TablePlayerCard[] playerSeats)
    {
        GameManager.instance.waitingFor.gameObject.SetActive(false);
        turnWinnerText.text = $"Turn winner: {winnerName}";
        winnerHandText.text = handType.ToString();
        turnWinnerText.transform.parent.gameObject.SetActive(true);

        Transform originalParentOfPlayerCards = playerSeats[0].transform.parent;
        
        foreach (TablePlayerCard playerCard in playerSeats)
        {
            playerCard.transform.SetParent(turnWinnerText.transform.parent);
            playerCard.transform.SetAsLastSibling();
            playerCard.transform.localScale = Vector3.one;
        }
        
        foreach (TablePlayerCard player in TableManager.instance.playerSeats)
        {
            player.ShowHand();
        }
        
        yield return new WaitForSeconds(15f);

        foreach (TablePlayerCard playerCard in playerSeats)
        {
            playerCard.transform.SetParent(originalParentOfPlayerCards);
            playerCard.transform.localScale = Vector3.one;
        }
        
        foreach (TablePlayerCard player in TableManager.instance.playerSeats)
        {
            player.ResetHand();
        }
        
        turnWinnerText.transform.parent.gameObject.SetActive(false);
    }
}
