using PokerParty_SharedDLL;
using UnityEngine;

public static class NetworkMessageManager
{
    public static void ProcessMesage(NetworkMessageType type, string data, int indexOfConnection)
    {
        switch (type)
        {
            case NetworkMessageType.ConnectionMessage:
                ConnectionMessage connectionData = JsonUtility.FromJson<ConnectionMessage>(data);
                LobbyGUI.Instance.DisplayNewPlayer(connectionData.connectedPlayer);
                break;

            case NetworkMessageType.DisconnectMessage:
                Debug.Log("Player got disconnected from the Host");

                DisconnectMessage disconnectMessage = JsonUtility.FromJson<DisconnectMessage>(data);
                LobbyGUI.Instance.RemovePlayerFromDisplay(disconnectMessage.disconnectedPlayer);

                RelayManager.Instance.DisconnectPlayer(indexOfConnection);
                break;

            case NetworkMessageType.ChatMessage:
                ChatMessage chatMessage = JsonUtility.FromJson<ChatMessage>(data);
                ChatGUI.Instance.AddChat(chatMessage);
                break;

            case NetworkMessageType.ReadyMessage:
                ReadyMessage readyMessage = JsonUtility.FromJson<ReadyMessage>(data);
                LobbyManager.Instance.ModifyPlayerReady(readyMessage);
                break;
        }
      
    }
}
