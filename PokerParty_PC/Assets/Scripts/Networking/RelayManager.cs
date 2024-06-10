using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.Threading.Tasks;
using TMPro;
using Unity.Networking.Transport.Relay;
using Unity.Networking.Transport;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using Unity.Collections;
using UnityEngine.Assertions;
using System;
using PokerParty_SharedDLL;
using Unity.Services.Relay.Models;

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance;

    [SerializeField] private Button createJoinCodeButton;

    private NetworkDriver networkDriver;
    private NativeList<NetworkConnection> Connections;

    private async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        await InitializeUnityServices();
        createJoinCodeButton.onClick.AddListener(() => CreateRelayAndJoinCode());
    }


    private void Update()
    {
        UpdateHost();    
    }

    void UpdateHost()
    {

        if (!networkDriver.IsCreated || !networkDriver.Bound)
        {
            return;
        }

        networkDriver.ScheduleUpdate().Complete();

        NetworkConnection incomingConnection;
        while ((incomingConnection = networkDriver.Accept()) != default(NetworkConnection))
        {
            Debug.Log("Accepted an incoming connection.");
            Connections.Add(incomingConnection);
        }

        // Process events from all connections.
        for (int i = 0; i < Connections.Length; i++)
        {
            Assert.IsTrue(Connections[i].IsCreated);

            // Resolve event queue.
            NetworkEvent.Type eventType;
            while ((eventType = networkDriver.PopEventForConnection(Connections[i], out var stream)) != NetworkEvent.Type.Empty)
            {
                switch (eventType)
                {
                    case NetworkEvent.Type.Data:
                        NetworkMessageType type = (NetworkMessageType)Enum.ToObject(typeof(NetworkMessageType), stream.ReadUInt());
                        FixedString512Bytes msg = stream.ReadFixedString512();
                        string data = msg.ToString();

                        Debug.Log($"Type: {type}");
                        Debug.Log($"Data received: {data}");

                        if (type == NetworkMessageType.ConnectionMessage)
                        {
                            ConnectionMessage connectionData = JsonUtility.FromJson<ConnectionMessage>(data);
                            LobbyGUI.Instance.DisplayNewPlayer(connectionData.connectedPlayer);
                            break;
                        }

                        if (type == NetworkMessageType.DisconnectMessage)
                        {
                            Debug.Log("Player got disconnected from the Host");

                            DisconnectMessage disconnectMessage = JsonUtility.FromJson<DisconnectMessage>(data);
                            LobbyGUI.Instance.RemovePlayerFromDisplay(disconnectMessage.disconnectedPlayer);

                            DisconnectPlayer(i);
                            break;
                        }

                        if (type == NetworkMessageType.ChatMessage)
                        {
                            ChatMessage chatMessage = JsonUtility.FromJson<ChatMessage>(data);
                            ChatGUI.Instance.AddChat(chatMessage);
                            break;
                        }

                        break;
                    case NetworkEvent.Type.Disconnect:
                        Debug.Log("AAAAAAAA");
                        break;
                }
            }
        }
    }

    public void SendMessageToPlayers(string msg)
    {
        if (Connections.Length == 0)
        {
            Debug.LogError("No players connected to send messages to.");
            return;
        }

        for (int i = 0; i < Connections.Length; i++)
        {
            if (networkDriver.BeginSend(Connections[i], out var writer) == 0)
            {
                writer.WriteFixedString32(msg);
                networkDriver.EndSend(writer);
            }
        }
    }

    private async Task InitializeUnityServices()
    {
        if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
        {
            await UnityServices.InitializeAsync();
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await SignInAnonymouslyAsync();
        }
    }

    async Task SignInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Sign in anonymously succeeded!");
            Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    private async void CreateRelayAndJoinCode(int maxConnections = 8)
    {
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);


            RelayServerData relayServerData = new RelayServerData(allocation, "udp");

            var settings = new NetworkSettings();
            settings.WithRelayParameters(ref relayServerData);

            if (networkDriver.IsCreated)
                networkDriver.Dispose();

            networkDriver = NetworkDriver.Create(settings);
            if (networkDriver.Bind(NetworkEndPoint.AnyIpv4) != 0)
            {
                Debug.LogError("Host client failed to bind");
            }
            else
            {
                if (networkDriver.Listen() != 0)
                {
                    Debug.LogError("Host client failed to listen");
                }
                else
                {
                    Debug.Log("Host client bound to Relay server");
                }
            }

            if (Connections.IsCreated)
                Connections.Dispose();

            Connections = new NativeList<NetworkConnection>(maxConnections, Allocator.Persistent);
            LobbyGUI.Instance.joinCodeText.text = joinCode;
            LobbyGUI.Instance.ShowPanel();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"Error creating join code: {e.Message}");
        }
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

    private void DisconnectPlayer(int index)
    {
        if (Connections[index].IsCreated)
        {
            Debug.Log("Player disconnected from host");
            networkDriver.Disconnect(Connections[index]);
            Connections[index] = default(NetworkConnection);
        }
        Connections.RemoveAt(index);
    }

    public void DeleteLobby()
    {
        Debug.Log("Lobby deleted");
        DisconnectAllPlayers();
        LobbyGUI.Instance.ClearDisplay();
        ChatGUI.Instance.ClearChat();

        if (networkDriver.IsCreated)
        {
            networkDriver.Dispose();
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Server app stopped");
        DeleteLobby();

        if (Connections.IsCreated)
        {
            Connections.Dispose();
        }
    }
}
