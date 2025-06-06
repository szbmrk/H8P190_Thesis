﻿using PokerParty_SharedDLL;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public static class MessageSender
{
    private static NetworkDriver networkDriver;
    private static NetworkConnection connection;

    public static void Initialize(NetworkDriver networkDriver, NetworkConnection connection)
    {
        MessageSender.networkDriver = networkDriver;
        MessageSender.connection = connection;
    }

    public static void SendMessageToHost(ANetworkMessageMobile message)
    {
        message.Player = PlayerManager.loggedInPlayer;

        if (!connection.IsCreated)
        {
            Logger.Log("Player isn't connected. No Host client to send message to.");
            return;
        }

        string messageInString = JsonUtility.ToJson(message);

        if (networkDriver.BeginSend(connection, out DataStreamWriter writer) != 0) return;
        
        writer.WriteUInt((uint)message.Type);
        writer.WriteFixedString512(messageInString);
        networkDriver.EndSend(writer);
        Logger.Log($"Message sent: {messageInString}");
    }
}