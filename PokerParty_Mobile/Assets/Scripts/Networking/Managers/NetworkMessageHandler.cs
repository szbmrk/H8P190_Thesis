using PokerParty_SharedDLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class NetworkMessageHandler
{
    public static void ProcessMesage(NetworkMessageType type, string data)
    {
        switch (type)
        {
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
            case NetworkMessageType.CommunityCardsChangedMessage:
                CommunityCardsChanged communityCardsChanged = FromStringToJson<CommunityCardsChanged>(data);
                CardsGUI.Instance.SetBestHandText(communityCardsChanged);
                break;
        }
    }

    private static T FromStringToJson<T>(string message)
    {
        return JsonUtility.FromJson<T>(message);
    }
}
