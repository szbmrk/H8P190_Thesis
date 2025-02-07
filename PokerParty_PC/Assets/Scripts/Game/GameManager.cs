using System;
using TMPro;
using UnityEngine;
using PokerParty_SharedDLL;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TextMeshProUGUI waitingFor;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        Loader.instance.StartLoading();
    }

    public void SetWaitingFor(string playerName)
    {
        waitingFor.gameObject.SetActive(true);
        waitingFor.text = $"Waiting for {playerName} ...";
    }
    
    public void HideWaitingFor()
    {
        waitingFor.gameObject.SetActive(false);
    }

    public void GameOver(TablePlayerCard winner)
    {
        TableManager.instance.RemovePlayersWith0Money();
        
        GameOverMessage gameOverMessage = new GameOverMessage();
        gameOverMessage.Place = 1;
        ConnectionManager.instance.SendMessageToConnection(
            ConnectionManager.instance.Connections[winner.indexInConnectionsArray], gameOverMessage);
        GameOverGUI.instance.Open(winner.TurnInfo.Player.PlayerName);
    }
}
