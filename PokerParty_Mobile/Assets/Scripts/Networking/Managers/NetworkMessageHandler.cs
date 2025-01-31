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
                NetworkingGUI.instance.ShowJoinedPanel(false);
                NetworkingGUI.instance.ResetReadyButton();
                PopupManager.instance.ShowPopup(PopupType.ErrorPopup, "Player with this name is already in the game");
                break;
            case NetworkMessageType.GameStartedMessage:
                SceneManager.LoadScene("Game");
                break;
            case NetworkMessageType.GameInfoMessage:
                GameInfoMessage gameInfoMessage = FromStringToJson<GameInfoMessage>(data);
                GameManager.instance.SetGameInfo(gameInfoMessage);
                break;
            case NetworkMessageType.DealCardsMessage:
                DealCardsMessage dealCardsMessage = FromStringToJson<DealCardsMessage>(data);
                GameManager.instance.SetCards(dealCardsMessage);
                break;
            case NetworkMessageType.YourTurnMessage:
                YourTurnMessage yourTurnMessage = FromStringToJson<YourTurnMessage>(data);
                GameManager.instance.StartTurn(yourTurnMessage);
                break;
            case NetworkMessageType.NotYourTurnMessage:
                NotYourTurnMessage notYourTurnMessage = FromStringToJson<NotYourTurnMessage>(data);
                //GameManager.instance.WaitingFor(notYourTurnMessage.PlayerInTurn);
                break;
            case NetworkMessageType.NewTurnStartedMessage:
                //NewTurnStartedMessage newTurnStartedMessage = FromStringToJson<NewTurnStartedMessage>(data);
                CardsGUI.instance.NewRoundStarted();
                break;
            case NetworkMessageType.CommunityCardsChangedMessage:
                CommunityCardsChangedMessage communityCardsChanged = FromStringToJson<CommunityCardsChangedMessage>(data);
                CardsGUI.instance.SetBestHandText(communityCardsChanged);
                break;
            case NetworkMessageType.RefreshedMoneyMessage:
                RefreshedMoneyMessage refreshedMoneyMessage = FromStringToJson<RefreshedMoneyMessage>(data);
                GameManager.instance.UpdateMoney(refreshedMoneyMessage.NewMoney);
                break;
            case NetworkMessageType.GameOverMessage:
                GameOverMessage gameOverMessage = FromStringToJson<GameOverMessage>(data);
                GameManager.instance.GameOver(gameOverMessage);
                break;
            case NetworkMessageType.GamePausedMessage:
                PauseMenu.instance.Pause();
                break;
            case NetworkMessageType.GameUnpausedMessage:
                PauseMenu.instance.Resume();
                break;
        }
    }

    private static T FromStringToJson<T>(string message)
    {
        return JsonUtility.FromJson<T>(message);
    }
}
