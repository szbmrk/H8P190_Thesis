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
                if (LobbyManager.Instance != null)
                {
                    if (!LobbyManager.Instance.CheckIfPlayerNameIsAlreadyInUse(connectionData.player, indexOfConnection))
                        LobbyManager.Instance.AddPlayer(connectionData.player, indexOfConnection);
                }
                break;

            case NetworkMessageType.DisconnectMessage:
                DisconnectMessage disconnectMessage = FromStringToJson<DisconnectMessage>(data);
                if (TableManager.Instance != null)
                    TableManager.Instance.PlayerDisconnected(disconnectMessage.player);
                if (LobbyManager.Instance != null)
                    LobbyManager.Instance.RemovePlayer(disconnectMessage.player);
                if (ConnectionManager.Instance != null)
                    ConnectionManager.Instance.DisconnectPlayer(indexOfConnection);
                break;

            case NetworkMessageType.ChatMessage:
                ChatMessage chatMessage = FromStringToJson<ChatMessage>(data);
                if (ChatGUI.Instance != null)
                    ChatGUI.Instance.AddChat(chatMessage);
                break;

            case NetworkMessageType.ReadyMessage:
                ReadyMessage readyMessage = FromStringToJson<ReadyMessage>(data);
                if (LobbyManager.Instance != null)
                    LobbyManager.Instance.ModifyPlayerReady(readyMessage);
                break;

            case NetworkMessageType.LoadedToGameMessage:
                LoadedToGameMessage loadedToGameMessage = FromStringToJson<LoadedToGameMessage>(data);
                if (TableManager.Instance != null)
                    TableManager.Instance.PlayerLoaded(loadedToGameMessage.player, indexOfConnection);
                break;
            case NetworkMessageType.TurnDoneMessage:
                TurnDoneMessage turnDoneMessage = FromStringToJson<TurnDoneMessage>(data);
                if (TableManager.Instance != null)
                    TableManager.Instance.PlayerTurnDone(turnDoneMessage);
                break;
        }
    }

    private static T FromStringToJson<T>(string message)
    {
        return JsonUtility.FromJson<T>(message);
    }
}
