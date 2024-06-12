using PokerParty_SharedDLL;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class MessageSender
{
    private static NetworkDriver networkDriver;
    private static NetworkConnection connection;

    public static void Initalize(NetworkDriver networkDriver, NetworkConnection connection)
    {
        MessageSender.networkDriver = networkDriver;
        MessageSender.connection = connection;
    }

    public static void SendPlayerDataToHost()
    {
        ConnectionMessage connectionMessage = new ConnectionMessage
        {
            player = PlayerManager.LoggedInPlayer
        };

        SendMessageToHost(connectionMessage);
    }

    public static void SendChatMessageToHost(string message)
    {
        ChatMessage chatMessage = new ChatMessage
        {
            player = PlayerManager.LoggedInPlayer,
            message = message
        };

        SendMessageToHost(chatMessage);
    }

    public static void SendDisconnectMessageToHost()
    {
        DisconnectMessage disconnectMessage = new DisconnectMessage
        {
            player = PlayerManager.LoggedInPlayer
        };

        SendMessageToHost(disconnectMessage);
    }

    public static void SendReadyMessageToHost(bool isReady)
    {
        ReadyMessage readyMessage = new ReadyMessage
        {
            player = PlayerManager.LoggedInPlayer,
            isReady = isReady
        };

        SendMessageToHost(readyMessage);
    }

    private static void SendMessageToHost(ANetworkMessage message)
    {
        if (!connection.IsCreated)
        {
            Debug.LogError("Player isn't connected. No Host client to send message to.");
            return;
        }

        string messageInString = JsonUtility.ToJson(message);

        if (networkDriver.BeginSend(connection, out DataStreamWriter writer) == 0)
        {
            writer.WriteUInt((uint)message.Type);
            writer.WriteFixedString512(messageInString);
            networkDriver.EndSend(writer);
            Debug.Log($"Message sent: {messageInString}");
        }
    }
}