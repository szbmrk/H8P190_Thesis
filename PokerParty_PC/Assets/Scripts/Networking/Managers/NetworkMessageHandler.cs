using PokerParty_SharedDLL;
using UnityEngine;

public static class NetworkMessageHandler
{
    public static void ProcessMesage(NetworkMessageType type, string data, int indexOfConnection)
    {
        switch (type)
        {
            case NetworkMessageType.ConnectionMessage:
                ConnectionMessage connectionData = FromStringToJson<ConnectionMessage>(data);
                LobbyManager.Instance.AddPlayer(connectionData.player, indexOfConnection);
                break;

            case NetworkMessageType.DisconnectMessage:
                DisconnectMessage disconnectMessage = FromStringToJson<DisconnectMessage>(data);
                if (LobbyManager.Instance != null)
                    LobbyManager.Instance.RemovePlayer(disconnectMessage.player);
                if (ConnectionManager.Instance != null)
                    ConnectionManager.Instance.DisconnectPlayer(indexOfConnection);
                break;

            case NetworkMessageType.ChatMessage:
                ChatMessage chatMessage = FromStringToJson<ChatMessage>(data);
                ChatGUI.Instance.AddChat(chatMessage);
                break;

            case NetworkMessageType.ReadyMessage:
                ReadyMessage readyMessage = FromStringToJson<ReadyMessage>(data);
                LobbyManager.Instance.ModifyPlayerReady(readyMessage);
                break;

            case NetworkMessageType.LoadedToGameMessage:
                LoadedToGameMessage loadedToGameMessage = FromStringToJson<LoadedToGameMessage>(data);
                TableManager.Instance.PlayerLoaded(loadedToGameMessage.player, indexOfConnection);
                break;
            case NetworkMessageType.TurnDoneMessage:
                TurnDoneMessage turnDoneMessage = FromStringToJson<TurnDoneMessage>(data);
                TableManager.Instance.PlayerTurnDone(turnDoneMessage);
                break;
        }
    }

    private static T FromStringToJson<T>(string message)
    {
        return JsonUtility.FromJson<T>(message);
    }
}
