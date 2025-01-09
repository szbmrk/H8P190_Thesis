using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TableGUI : MonoBehaviour
{
    public static TableGUI instance;
    public List<Transform> seatPositions = new List<Transform>();
    public List<Transform> cardPositions = new List<Transform>();

    [SerializeField] private TextMeshProUGUI moneyInPotText;
    [SerializeField] private TextMeshProUGUI turnWinnerText;

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

    public IEnumerator showTurnWinner(string winnerName)
    {
        GameManager.instance.waitingFor.gameObject.SetActive(false);
        turnWinnerText.text = $"Turn winner: {winnerName}";
        turnWinnerText.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        turnWinnerText.transform.parent.gameObject.SetActive(false);
    }
}
