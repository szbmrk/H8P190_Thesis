using PokerParty_SharedDLL;
using UnityEngine;

public static class NetworkMessageHandler
{
    public static void ProcessMessage(NetworkMessageType type, string data, int indexOfConnection)
    {
        switch (type)
        {
            case NetworkMessageType.ConnectionMessage:
                ConnectionMessage connectionData = FromStringToJson<ConnectionMessage>(data);
                if (LobbyManager.instance != null)
                    if (!LobbyManager.instance.CheckIfPlayerNameIsAlreadyInUse(connectionData.Player, indexOfConnection))
                        LobbyManager.instance.AddPlayer(connectionData.Player, indexOfConnection);
                if (GameManager.instance != null)
                {
                    ConnectionManager.instance.SendMessageToConnection(ConnectionManager.instance.Connections[indexOfConnection], new GameAlreadyStartedMessage());
                    ConnectionManager.instance.DisconnectPlayer(indexOfConnection);
                }
                break;

            case NetworkMessageType.ChatMessage:
                ChatMessage chatMessage = FromStringToJson<ChatMessage>(data);
                if (ChatGUI.instance != null)
                    ChatGUI.instance.AddChat(chatMessage);
                break;

            case NetworkMessageType.ReadyMessage:
                ReadyMessage readyMessage = FromStringToJson<ReadyMessage>(data);
                if (LobbyManager.instance != null)
                    LobbyManager.instance.ModifyPlayerReady(readyMessage);
                break;

            case NetworkMessageType.LoadedToGameMessage:
                LoadedToGameMessage loadedToGameMessage = FromStringToJson<LoadedToGameMessage>(data);
                if (TableManager.instance != null)
                    TableManager.instance.PlayerLoaded(loadedToGameMessage.Player, indexOfConnection);
                break;
            case NetworkMessageType.TurnDoneMessage:
                TurnDoneMessage turnDoneMessage = FromStringToJson<TurnDoneMessage>(data);
                if (TableManager.instance != null)
                    TableManager.instance.PlayerTurnDone(turnDoneMessage);
                break;
        }
    }

    private static T FromStringToJson<T>(string message)
    {
        return JsonUtility.FromJson<T>(message);
    }
}
