using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Networking.Transport.Relay;
using Unity.Networking.Transport;
using Unity.Services.Relay.Models;
using Unity.Collections;
using System.Threading.Tasks;
using PokerParty_SharedDLL;
using System;
using System.Collections;

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance;

    static bool ReadyToQuit;

    private NetworkDriver networkDriver;
    private NetworkConnection connection;

    private string joinCode;

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
    }

    private void Update()
    {
        if (connection != null)
        {
            UpdatePlayer();
        }
    }

    void UpdatePlayer()
    {
        if (!networkDriver.IsCreated || !networkDriver.Bound)
        {
            return;
        }

        networkDriver.ScheduleUpdate().Complete();

        NetworkEvent.Type eventType;
        while ((eventType = connection.PopEvent(networkDriver, out var stream)) != NetworkEvent.Type.Empty)
        {
            switch (eventType)
            {
                case NetworkEvent.Type.Data:
                    FixedString32Bytes msg = stream.ReadFixedString32();
                    Debug.Log($"Player received msg: {msg}");
                    break;

                case NetworkEvent.Type.Connect:
                    Debug.Log("Player connected to the Host");
                    SendPlayerDataToHost();
                    NetworkingGUI.Instance.ShowJoinedPanel(true);
                    break;

                case NetworkEvent.Type.Disconnect:
                    Debug.Log("Player got disconnected from the Host");
                    connection = default(NetworkConnection);
                    NetworkingGUI.Instance.ShowJoinedPanel(false);
                    break;
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

    public async void JoinRelay(string joinCode)
    {
        try
        {
            this.joinCode = joinCode.Trim();
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(this.joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "udp");
            var settings = new NetworkSettings();
            settings.WithRelayParameters(ref relayServerData);

            if (networkDriver.IsCreated)
            {
                networkDriver.Dispose();
            }

            networkDriver = NetworkDriver.Create(settings);

            BindToHostAndSendPlayerData(networkDriver);
        }
        catch (RelayServiceException e)
        {
            NetworkingGUI.Instance.joinBtn.interactable = true;
            StartCoroutine(NetworkingGUI.Instance.DisplayErrorText(e.Message));
        }
    }

    private void BindToHostAndSendPlayerData(NetworkDriver networkDriver)
    {
        if (networkDriver.Bind(NetworkEndPoint.AnyIpv4) != 0)
        {
            NetworkingGUI.Instance.joinBtn.interactable = true;
            StartCoroutine(NetworkingGUI.Instance.DisplayErrorText("Failed to bind to any IP"));
        }
        else
        {
            Debug.Log("Player client bound to Relay server");
        }

        connection = networkDriver.Connect();

        Debug.Log("Joined Relay with Join Code: " + joinCode);
    }

    private void SendPlayerDataToHost()
    {
        ConnectionMessage connectionMessage = new ConnectionMessage
        {
            connectedPlayer = PlayerManager.LoggedInPlayer
        };


        string connectionMessageInString = JsonUtility.ToJson(connectionMessage);

        SendMessageToHost(NetworkMessageType.ConnectionMessage, connectionMessageInString);
    }

    public void SendChatMessageToHost(string message)
    {
        ChatMessage chatMessage = new ChatMessage
        {
            player = PlayerManager.LoggedInPlayer,
            message = message
        };

        string simpleMessageInString = JsonUtility.ToJson(chatMessage);

        SendMessageToHost(NetworkMessageType.ChatMessage, simpleMessageInString);
    }

    private void SendMessageToHost(NetworkMessageType type, FixedString512Bytes msg)
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

    public void DisconnectFromHost()
    {
        if (connection.IsCreated)
        {
            SendDisconnectMessageToHost();
            StartCoroutine(DisposeNetworkDriver());
        }
    }

    private void SendDisconnectMessageToHost()
    {
        DisconnectMessage disconnectMessage = new DisconnectMessage
        {
            disconnectedPlayer = PlayerManager.LoggedInPlayer
        };


        string disconnectMessageInString = JsonUtility.ToJson(disconnectMessage);

        SendMessageToHost(NetworkMessageType.DisconnectMessage, disconnectMessageInString);
    }

    static bool WantsToQuit()
    {
        if (!Instance.networkDriver.IsCreated)
            return true;

        Instance.StartCoroutine(Instance.StartQutiting());

        return ReadyToQuit;
    }

    IEnumerator DisposeNetworkDriver()
    {
        yield return new WaitForSeconds(1f);
        if (networkDriver.IsCreated)
        {
            networkDriver.Dispose();
        }
    }

    IEnumerator StartQutiting()
    {
        DisconnectFromHost();

        yield return DisposeNetworkDriver();

        ReadyToQuit = true;
        Debug.Log("Client app stopped");
        Application.Quit();
    }

    [RuntimeInitializeOnLoadMethod]
    static void RunOnStart()
    {
        Application.wantsToQuit += WantsToQuit;
    }
}
