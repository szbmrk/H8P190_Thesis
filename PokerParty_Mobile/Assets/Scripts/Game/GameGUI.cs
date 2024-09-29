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

    public TextMeshProUGUI moneyTex;
    public GameObject waitingFor;
    public TextMeshProUGUI waitingForTex;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void WaitingFor(string playerName)
    {
        waitingFor.SetActive(true);
        waitingForTex.text = $"Waiting for \"{playerName}\" ...";
    }

    public void StartTurn()
    {
        waitingFor.SetActive(false);
    }

}
