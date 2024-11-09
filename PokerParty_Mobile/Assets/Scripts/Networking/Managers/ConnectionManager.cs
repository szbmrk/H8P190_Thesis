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
using UnityEngine.SceneManagement;
using PokerParty_SharedDLL;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Instance;

    public NetworkDriver networkDriver;
    private NetworkConnection connection;

    private string joinCode;

    private async void Awake()
    {
        if (Instance == null)
        { 
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        await AuthAndServicesManager.InitializeUnityServices();
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
                    NetworkMessageType type = (NetworkMessageType)Enum.ToObject(typeof(NetworkMessageType), stream.ReadUInt());

                    FixedString512Bytes msg = stream.ReadFixedString512();
                    string data = msg.ToString();

                    Debug.Log($"Data received: {data}");

                    NetworkMessageHandler.ProcessMesage(type, data);
                    break;

                case NetworkEvent.Type.Connect:
                    Debug.Log("Player connected to the Host");
                    MessageSender.SendMessageToHost(new ConnectionMessage());
                    NetworkingGUI.Instance.ShowJoinedPanel(true);
                    break;

                case NetworkEvent.Type.Disconnect:
                    connection = default(NetworkConnection);
                    if (NetworkingGUI.Instance != null)
                    {
                        NetworkingGUI.Instance.ShowJoinedPanel(false);
                        NetworkingGUI.Instance.ResetReadyButton();
                        PopupManager.Instance.ShowPopup(PopupType.ErrorPopup, "You got disconnected from the game");
                    }

                    if (GameOverGUI.Instance != null)
                    {
                        GameOverGUI.Instance.ShowGameOverPanel();
                    }


                    StartCoroutine(DisposeNetworkDriver());
                    break;
            }
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
        if (networkDriver.Bind(NetworkEndpoint.AnyIpv4) != 0)
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
            MessageSender.SendMessageToHost(new DisconnectMessage());
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
