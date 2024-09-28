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

    public static void SendMessageToHost(ANetworkMessageMobile message)
    {
        message.player = PlayerManager.LoggedInPlayer;

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