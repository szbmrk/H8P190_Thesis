using TMPro;
using UnityEngine;

public class GameGUI : MonoBehaviour
{
    public static GameGUI instance;

    public TextMeshProUGUI moneyText;
    public GameObject waitingFor;
    public TextMeshProUGUI waitingForText;
    public GameObject inGamePanel;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void WaitingFor(string playerName)
    {
        waitingFor.SetActive(true);
        waitingForText.text = $"Waiting for \"{playerName}\" ...";
    }

    public void StartTurn()
    {
        waitingFor.SetActive(false);
    }

    public void StartGame()
    {
        Loader.instance.StopLoading();
        inGamePanel.SetActive(true);
    }

    public void UpdateMoney()
    {
        moneyText.text = $"{GameManager.instance.money} $";
    }
}
