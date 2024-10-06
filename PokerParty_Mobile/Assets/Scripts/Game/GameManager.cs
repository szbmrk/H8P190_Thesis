using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PokerParty_SharedDLL;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject inGamePanel;

    public Card[] cards = new Card[2];

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
        ActionManager.Instance.EnableActions(yourTurnMessage.PossibleActions);
    }

    public void SetGameInfo(GameInfoMessage gameInfo)
    {
        Loader.Instance.StopLoading();
        GameGUI.Instance.SetGameInfo(gameInfo);
    }

    public void SetCards(DealCardsMessage dealCards)
    {
        cards = dealCards.Cards;
        CardsGUI.Instance.SetCards(cards);
    }
}
