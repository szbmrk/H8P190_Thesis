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

    public void SetWaitingFor(string playerName)
    {
        waitingFor.gameObject.SetActive(true);
        waitingFor.text = $"Waiting for {playerName} ...";
    }

    public void SendGameOverMessageToPlayer(int indexInConnectionsArray)
    {
        GameOverMessage gameOverMessage = new GameOverMessage();
        ConnectionManager.instance.SendMessageToConnection(ConnectionManager.instance.Connections[indexInConnectionsArray], gameOverMessage);
    }

    public void GameOver(string winnerName)
    {
        GameOverMessage gameOverMessage = new GameOverMessage();
        ConnectionManager.instance.SendMessageToAllConnections(gameOverMessage);
        GameOverGUI.instance.Open(winnerName);
    }
}
