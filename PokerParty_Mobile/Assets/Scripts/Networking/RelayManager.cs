using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Networking.Transport.Relay;
using Unity.Networking.Transport;
using Unity.Services.Relay.Models;
using Unity.Collections;
using System.Threading.Tasks;
using System.Collections;
using System;

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance;

    public NetworkDriver networkDriver;
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
                    MessageSender.SendPlayerDataToHost();
                    NetworkingGUI.Instance.ShowJoinedPanel(true);
                    break;

                case NetworkEvent.Type.Disconnect:
                    connection = default(NetworkConnection);
                    NetworkingGUI.Instance.ShowJoinedPanel(false);
                    PopupManager.Instance.ShowPopup(PopupType.BasicPopup, "You got disconnected from the game");
                    StartCoroutine(DisposeNetworkDriver());
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

            if (string.IsNullOrEmpty(this.joinCode))
            {
                throw new Exception("Join code is empty");
            }

            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(this.joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "udp");
            var settings = new NetworkSettings();
            settings.WithRelayParameters(ref relayServerData);

            if (networkDriver.IsCreated)
            {
                networkDriver.Dispose();
            }

            networkDriver = NetworkDriver.Create(settings);

            BindToHost(networkDriver);
        }
        catch (Exception e)
        {
            NetworkingGUI.Instance.ShowJoinError(e.Message);
        }
    }

    private void BindToHost(NetworkDriver networkDriver)
    {
        if (networkDriver.Bind(NetworkEndPoint.AnyIpv4) != 0)
        {
            throw new NetworkBindingException("Failed to bind to any IP");
        }

        Debug.Log("Player client bound to Relay server");

        connection = networkDriver.Connect();
        MessageSender.Initalize(networkDriver, connection);

        Debug.Log("Joined Relay with Join Code: " + joinCode);
    }

    public void DisconnectFromHost()
    {
        if (connection.IsCreated)
        {
            MessageSender.SendDisconnectMessageToHost();
        }
    }

    public IEnumerator DisposeNetworkDriver()
    {
        yield return new WaitForSeconds(1f);
        if (networkDriver.IsCreated)
        {
            networkDriver.Dispose();
        }
    }
}
