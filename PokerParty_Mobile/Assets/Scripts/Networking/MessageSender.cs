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
            connectedPlayer = PlayerManager.LoggedInPlayer
        };

        string connectionMessageInString = JsonUtility.ToJson(connectionMessage);

        SendMessageToHost(NetworkMessageType.ConnectionMessage, connectionMessageInString);
    }

    public static void SendChatMessageToHost(string message)
    {
        ChatMessage chatMessage = new ChatMessage
        {
            player = PlayerManager.LoggedInPlayer,
            message = message
        };

        string simpleMessageInString = JsonUtility.ToJson(chatMessage);

        SendMessageToHost(NetworkMessageType.ChatMessage, simpleMessageInString);
    }

    public static void SendDisconnectMessageToHost()
    {
        DisconnectMessage disconnectMessage = new DisconnectMessage
        {
            disconnectedPlayer = PlayerManager.LoggedInPlayer
        };


        string disconnectMessageInString = JsonUtility.ToJson(disconnectMessage);

        SendMessageToHost(NetworkMessageType.DisconnectMessage, disconnectMessageInString);
    }

    public static void SendReadyMessageToHost(bool isReady)
    {
        ReadyMessage readyMessage = new ReadyMessage
        {
            player = PlayerManager.LoggedInPlayer,
            isReady = isReady
        };

        string readyMessageInString = JsonUtility.ToJson(readyMessage);

        SendMessageToHost(NetworkMessageType.ReadyMessage, readyMessageInString);
    }

    private static void SendMessageToHost(NetworkMessageType type, FixedString512Bytes msg)
    {
        if (!connection.IsCreated)
        {
            Debug.LogError("Player isn't connected. No Host client to send message to.");
            return;
        }

        if (networkDriver.BeginSend(connection, out DataStreamWriter writer) == 0)
        {
            writer.WriteUInt((uint)type);
            writer.WriteFixedString512(msg);
            networkDriver.EndSend(writer);
            Debug.Log($"Message sent: {msg}");
        }
    }
}