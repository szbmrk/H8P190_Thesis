using PokerParty_SharedDLL;
using UnityEngine;

public static class NetworkMessageManager
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
                Debug.Log("Player got disconnected from the Host");

                DisconnectMessage disconnectMessage = FromStringToJson<DisconnectMessage>(data);

                LobbyManager.Instance.RemovePlayer(disconnectMessage.player);
                RelayManager.Instance.DisconnectPlayer(indexOfConnection);
                break;

            case NetworkMessageType.ChatMessage:
                ChatMessage chatMessage = FromStringToJson<ChatMessage>(data);
                ChatGUI.Instance.AddChat(chatMessage);
                break;

            case NetworkMessageType.ReadyMessage:
                ReadyMessage readyMessage = FromStringToJson<ReadyMessage>(data);
                LobbyManager.Instance.ModifyPlayerReady(readyMessage);
                break;
        }
    }

    private static T FromStringToJson<T>(string message)
    {
        return JsonUtility.FromJson<T>(message);
    }
}
