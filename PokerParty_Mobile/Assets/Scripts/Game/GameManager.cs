using System.Collections;
using UnityEngine;
using PokerParty_SharedDLL;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Card[] cards = new Card[2];
    public int money;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        Loader.instance.StartLoading();
        MessageSender.SendMessageToHost(new LoadedToGameMessage());
    }

    public void StartTurn(YourTurnMessage yourTurnMessage)
    {
        Settings.moneyNeededToCall = yourTurnMessage.MoneyNeededToCall;
        ActionManager.instance.EnableActions(yourTurnMessage.PossibleActions);
        CardsGUI.instance.yourTurnText.SetActive(true);
        Handheld.Vibrate();
    }

    public void SetGameInfo(GameInfoMessage gameInfo)
    {
        UpdateMoney(gameInfo.StartingMoney);
        Settings.SetSettings(gameInfo);
        GameGUI.instance.StartGame();
    }

    public void UpdateMoney(int money)
    {
        this.money = money;
        GameGUI.instance.UpdateMoney();
    }

    public void SetCards(DealCardsMessage dealCards)
    {
        cards = dealCards.Cards;
        CardsGUI.instance.SetCards(cards);
        Handheld.Vibrate();
    }

    public void GameOver(GameOverMessage gameOverMessage)
    {
        ActionManager.instance.DisableActions();
        GameOverGUI.instance.ShowGameOverPanel(gameOverMessage.Place);
    }

    public IEnumerator DisconnectFromGame()
    {
        Loader.instance.StartLoading();
        ConnectionManager.instance.DisconnectFromHost();
        yield return ConnectionManager.instance.DisposeNetworkDriver();
        Destroy(ConnectionManager.instance.gameObject);
        SceneManager.sceneLoaded += OnLobbySceneLoaded;
        SceneManager.LoadScene("Lobby");
    }
    
    public IEnumerator GoBackToMainMenu()
    {
        Loader.instance.StartLoading();
        ConnectionManager.instance.DisconnectFromHost();
        yield return ConnectionManager.instance.DisposeNetworkDriver();
        Destroy(ConnectionManager.instance.gameObject);
        SceneManager.LoadScene("Lobby");
    }
    
    private void OnLobbySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Lobby") return;
        
        PopupManager.instance.ShowPopup(PopupType.ErrorPopup, "You got disconnected from the game");
        SceneManager.sceneLoaded -= OnLobbySceneLoaded;
    }
}
