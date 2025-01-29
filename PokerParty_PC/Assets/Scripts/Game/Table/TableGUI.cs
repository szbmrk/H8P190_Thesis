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

    public IEnumerator showTurnWinner(string winnerName, HandType handType)
    {
        GameManager.instance.waitingFor.gameObject.SetActive(false);
        turnWinnerText.text = $"Turn winner: {winnerName}";
        winnerHandText.text = handType.ToString();
        turnWinnerText.transform.parent.gameObject.SetActive(true);

        foreach (TablePlayerCard player in TableManager.instance.playerSeats)
        {
            player.ShowHand();
        }
        
        yield return new WaitForSeconds(5f);
        
        foreach (TablePlayerCard player in TableManager.instance.playerSeats)
        {
            player.ResetHand();
        }
        
        turnWinnerText.transform.parent.gameObject.SetActive(false);
    }
}
