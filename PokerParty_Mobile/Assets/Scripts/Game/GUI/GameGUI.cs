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

    public void StartTurn()
    {
        waitingFor.SetActive(false);
    }

    public void StartGame()
    {
        Loader.Instance.StopLoading();
        inGamePanel.SetActive(true);
    }

    public void UpdateMoney()
    {
        moneyText.text = $"{GameManager.Instance.money} $";
    }
}
