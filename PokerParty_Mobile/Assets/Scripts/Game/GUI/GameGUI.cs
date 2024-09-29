using PokerParty_SharedDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class GameGUI : MonoBehaviour
{
    public static GameGUI Instance;

    public TextMeshProUGUI moneyText;
    public GameObject waitingFor;
    public TextMeshProUGUI waitingForText;
    public GameObject inGamePanel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void WaitingFor(string playerName)
    {
        waitingFor.SetActive(true);
        waitingForText.text = $"Waiting for \"{playerName}\" ...";
    }

    public void StartTurn(YourTurnMessage yourTurnMessage)
    {
        waitingFor.SetActive(false);
    }

    public void SetGameInfo(GameInfoMessage gameInfo)
    {
        moneyText.text = $"{gameInfo.StartingMoney} $";
        Loader.Instance.StopLoading();
        inGamePanel.SetActive(true);
    }
}
