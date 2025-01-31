using System.Collections;
using System.Collections.Generic;
using PokerParty_SharedDLL;
using TMPro;
using UnityEngine;

public class TableGUI : MonoBehaviour
{
    public static TableGUI instance;
    public List<Transform> seatPositions = new List<Transform>();
    public List<Transform> cardPositions = new List<Transform>();

    [SerializeField] private TextMeshProUGUI moneyInPotText;
    [SerializeField] private TextMeshProUGUI turnWinnerText;
    [SerializeField] private TextMeshProUGUI winnerHandText;

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

    public void DisplayCard(TableCard card, int i)
    {
        card.transform.position = cardPositions[i].position;
        card.gameObject.SetActive(true);
    }

    public void RefreshMoneyInPotText(int potMoney)
    {
        moneyInPotText.text = $"Pot: {potMoney} $";
    }

    public IEnumerator ShowTurnWinner(string winnerName, HandType handType, TablePlayerCard[] playerSeats)
    {
        GameManager.instance.waitingFor.gameObject.SetActive(false);
        turnWinnerText.text = $"Turn winner: {winnerName}";
        winnerHandText.text = handType.ToString();
        turnWinnerText.transform.parent.gameObject.SetActive(true);

        List<TablePlayerCard> toDelete = new List<TablePlayerCard>(); 
        
        foreach (TablePlayerCard playerCard in playerSeats)
        {
            TablePlayerCard newPlayerCard = Instantiate(playerCard, playerCard.transform.position, Quaternion.identity, playerCard.transform.parent);
            newPlayerCard.transform.SetParent(turnWinnerText.transform.parent);
            newPlayerCard.transform.SetAsLastSibling();
            newPlayerCard.transform.localScale = Vector3.one;
            
            toDelete.Add(newPlayerCard);
        }
        
        foreach (TablePlayerCard player in TableManager.instance.playerSeats)
        {
            player.ShowHand();
        }
        
        yield return new WaitForSeconds(15f);

        foreach (TablePlayerCard playerCard in toDelete)
        {
            Destroy(playerCard.gameObject);
        }
        
        foreach (TablePlayerCard player in TableManager.instance.playerSeats)
        {
            player.ResetHand();
        }
        
        turnWinnerText.transform.parent.gameObject.SetActive(false);
    }
}
