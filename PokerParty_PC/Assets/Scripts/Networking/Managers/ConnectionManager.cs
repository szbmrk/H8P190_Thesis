﻿using PokerParty_SharedDLL;
using System;
using System.Collections;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Assertions;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager instance;

    public NetworkDriver NetworkDriver;
    public NativeArray<NetworkConnection> Connections;

    private async void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        await AuthAndServicesManager.InitializeUnityServices();
        CreateRelayAndJoinCode();
    }

    private void Update()
    {
        UpdateHost();
    }

    private void UpdateHost()
    {
        if (!NetworkDriver.IsCreated || !NetworkDriver.Bound )
        {
            return;
        }

        NetworkDriver.ScheduleUpdate().Complete();

        NetworkConnection incomingConnection;
        while ((incomingConnection = NetworkDriver.Accept()) != default(NetworkConnection))
        {
            Logger.Log("Client connected to host");
            AddToConnections(incomingConnection);
        }

        // Process events from all connections.
        for (int i = 0; i < Connections.Length; i++)
        {
            if (!Connections[i].IsCreated) continue;

            NetworkEvent.Type eventType;
            while ((eventType = NetworkDriver.PopEventForConnection(Connections[i], out var stream)) != NetworkEvent.Type.Empty)
            {
                switch (eventType)
                {
                    case NetworkEvent.Type.Disconnect:
                        Logger.Log("Client disconnected from host");
                        AudioManager.instance.disconnectedSource.Play();
                        
                        if (TableManager.instance != null)
                            TableManager.instance.PlayerDisconnected(TableManager.instance.GetPLayerByIndexInConnectionsArray(i));
                        if (LobbyManager.instance != null)
                            LobbyManager.instance.RemovePlayer(LobbyManager.instance.GetPlayerByIndexInConnectionsArray(i));
                        
                        DisconnectPlayer(i);
                        break;
                    
                    case NetworkEvent.Type.Data:
                        NetworkMessageType type = (NetworkMessageType)Enum.ToObject(typeof(NetworkMessageType), stream.ReadUInt());

                        FixedString512Bytes msg = stream.ReadFixedString512();
                        string data = msg.ToString();

                        Logger.Log($"Data received: type = {type}, data = {data}");

                        NetworkMessageHandler.ProcessMessage(type, data, i);
                        break;
                }
            }
        }
    }
    
    private void AddToConnections(NetworkConnection connection)
    {
        for (int i = 0; i < Connections.Length; i++)
        {
            if (Connections[i].IsCreated) continue;
            
            Connections[i] = connection;
            return;
        }
    }

    private async void CreateRelayAndJoinCode(int maxConnections = 8)
    {
        try
        {
            Loader.instance.StartLoading();

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            BindTheHost(allocation, maxConnections);

            LobbyGUI.instance.joinCodeText.text = joinCode;
            LobbyGUI.instance.ShowPanel();
            Loader.instance.StopLoading();
            
            AudioManager.instance.lobbyLoadedSource.Play();
            
            Logger.Log($"Lobby created with join code: {joinCode}");
        }
        catch (Exception e)
        {
            Loader.instance.StopLoading();
            PopupManager.instance.ShowPopup(PopupType.ErrorPopup, e.Message);
        }
    }

    private void BindTheHost(Allocation allocation, int maxConnections)
    {
        RelayServerData relayServerData = allocation.ToRelayServerData("udp");

        var settings = new NetworkSettings();
        settings.WithRelayParameters(ref relayServerData);

        if (NetworkDriver.IsCreated)
            NetworkDriver.Dispose();

        NetworkDriver = NetworkDriver.Create(settings);
        if (NetworkDriver.Bind(NetworkEndpoint.AnyIpv4) != 0)
        {
            throw new Exception("Host client failed to bind");
        }
        
        if (NetworkDriver.Listen() != 0)
        {
            throw new Exception("Host client failed to listen");
        }
        
        Logger.Log("Host client bound to Relay server");

        if (Connections.IsCreated)
            Connections.Dispose();

        Connections = new NativeArray<NetworkConnection>(maxConnections, Allocator.Persistent);
    }

    public void DisconnectAllPlayers()
    {
        if (Connections.Length == 0)
        {
            Debug.LogError("No players connected to disconnect.");
            return;
        }

        for (int i = Connections.Length - 1; i >= 0; i--)
        {
            DisconnectPlayer(i);
        }
    }

    public void DisconnectPlayer(int index)
    {
        if (!Connections[index].IsCreated) return;
        
        Logger.Log("Player disconnected from host");
        NetworkDriver.Disconnect(Connections[index]);
        Connections[index] = default(NetworkConnection);
    }

    public void SendMessageToAllConnections(ANetworkMessagePc message)
    {
        foreach (NetworkConnection connection in Connections)
        {
            SendMessageToConnection(connection, message);
        }
    }

    public void SendMessageToConnection(NetworkConnection connection, ANetworkMessagePc message)
    {
        string messageInString = JsonUtility.ToJson(message);

        if (NetworkDriver.BeginSend(connection, out DataStreamWriter writer) != 0) return;
        
        writer.WriteUInt((uint)message.Type);
        writer.WriteFixedString512(messageInString);
        NetworkDriver.EndSend(writer);
        Logger.Log($"Message sent: {messageInString}");
    }

    private IEnumerator DisposeDriver()
    {
        if (!NetworkDriver.IsCreated) yield break;
        yield return new WaitForSeconds(1f);
        
        Logger.Log("Host disposed");
        NetworkDriver.Dispose();
    }

    private IEnumerator DisposeConnections()
    {
        if (!Connections.IsCreated) yield break;
        yield return new WaitForSeconds(1f);

        Logger.Log("Connections disposed");
        Connections.Dispose();
    }

    public IEnumerator DisposeDriverAndConnections()
    {
        yield return DisposeDriver();
        yield return DisposeConnections();
    }
}