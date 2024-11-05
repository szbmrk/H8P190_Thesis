using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PokerParty_SharedDLL;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject inGamePanel;

    public Card[] cards = new Card[2];
    public int money;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        Loader.Instance.StartLoading();
        MessageSender.SendMessageToHost(new LoadedToGameMessage());
    }

    public void WaitingFor(string playerName)
    {
        GameGUI.Instance.WaitingFor(playerName);
    }

    public void StartTurn(YourTurnMessage yourTurnMessage)
    {
        GameGUI.Instance.StartTurn();
        Settings.MoneyNeededToCall = yourTurnMessage.moneyNeededToCall;
        ActionManager.Instance.EnableActions(yourTurnMessage.possibleActions);
    }

    public void SetGameInfo(GameInfoMessage gameInfo)
    {
        UpdateMoney(gameInfo.startingMoney);
        Settings.SetSettings(gameInfo);
        GameGUI.Instance.StartGame();
    }

    public void UpdateMoney(int money)
    {
        this.money = money;
        GameGUI.Instance.UpdateMoney();
    }

    public void SetCards(DealCardsMessage dealCards)
    {
        cards = dealCards.cards;
        CardsGUI.Instance.SetCards(cards);
    }

    public void GameOver()
    {
        ActionManager.Instance.DisableActions();
        GameOverGUI.Instance.ShowGameOverPanel();
    }
}
