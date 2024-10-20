using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TextMeshProUGUI waitingFor;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void SetWaitingFor(string playerName)
    {
        waitingFor.gameObject.SetActive(true);
        waitingFor.text = $"Waiting for {playerName} ...";
    }

    public void GameOver()
    {

    }
}
