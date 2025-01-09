using PokerParty_SharedDLL;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class NetworkMessageHandler
{
    public static void ProcessMessage(NetworkMessageType type, string data)
    {
        switch (type)
        {
            case NetworkMessageType.PlayerNameAlreadyInUseMessage:
                NetworkingGUI.Instance.ShowJoinedPanel(false);
                NetworkingGUI.Instance.ResetReadyButton();
                PopupManager.Instance.ShowPopup(PopupType.ErrorPopup, "Player with this name is already in the game");
                break;
            case NetworkMessageType.GameStartedMessage:
                SceneManager.LoadScene("Game");
                break;
            case NetworkMessageType.GameInfoMessage:
                GameInfoMessage gameInfoMessage = FromStringToJson<GameInfoMessage>(data);
                GameManager.Instance.SetGameInfo(gameInfoMessage);
                break;
            case NetworkMessageType.DealCardsMessage:
                DealCardsMessage dealCardsMessage = FromStringToJson<DealCardsMessage>(data);
                GameManager.Instance.SetCards(dealCardsMessage);
                break;
            case NetworkMessageType.YourTurnMessage:
                YourTurnMessage yourTurnMessage = FromStringToJson<YourTurnMessage>(data);
                GameManager.Instance.StartTurn(yourTurnMessage);
                break;
            case NetworkMessageType.NotYourTurnMessage:
                NotYourTurnMessage notYourTurnMessage = FromStringToJson<NotYourTurnMessage>(data);
                GameManager.Instance.WaitingFor(notYourTurnMessage.playerInTurn);
                break;
            case NetworkMessageType.NewTurnStartedMessage:
                NewTurnStartedMessage newTurnStartedMessage = FromStringToJson<NewTurnStartedMessage>(data);
                CardsGUI.Instance.NewRoundStarted();
                break;
            case NetworkMessageType.CommunityCardsChangedMessage:
                CommunityCardsChanged communityCardsChanged = FromStringToJson<CommunityCardsChanged>(data);
                CardsGUI.Instance.SetBestHandText(communityCardsChanged);
                break;
            case NetworkMessageType.RefreshedMoneyMessage:
                RefreshedMoneyMessage refreshedMoneyMessage = FromStringToJson<RefreshedMoneyMessage>(data);
                GameManager.Instance.UpdateMoney(refreshedMoneyMessage.newMoney);
                break;
            case NetworkMessageType.GameOverMessage:
                GameOverMessage gameOverMessage = FromStringToJson<GameOverMessage>(data);
                GameManager.Instance.GameOver();
                break;
        }
    }

    private static T FromStringToJson<T>(string message)
    {
        return JsonUtility.FromJson<T>(message);
    }
}
