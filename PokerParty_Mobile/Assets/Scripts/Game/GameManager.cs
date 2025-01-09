using UnityEngine;
using PokerParty_SharedDLL;

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

    public void WaitingFor(string playerName)
    {
        GameGUI.instance.WaitingFor(playerName);
    }

    public void StartTurn(YourTurnMessage yourTurnMessage)
    {
        GameGUI.instance.StartTurn();
        Settings.moneyNeededToCall = yourTurnMessage.moneyNeededToCall;
        ActionManager.Instance.EnableActions(yourTurnMessage.possibleActions);
    }

    public void SetGameInfo(GameInfoMessage gameInfo)
    {
        UpdateMoney(gameInfo.startingMoney);
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
        cards = dealCards.cards;
        CardsGUI.instance.SetCards(cards);
    }

    public void GameOver()
    {
        ActionManager.Instance.DisableActions();
        GameOverGUI.instance.ShowGameOverPanel();
    }
}
