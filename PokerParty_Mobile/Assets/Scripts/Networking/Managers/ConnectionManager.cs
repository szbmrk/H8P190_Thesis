using UnityEngine;
using Unity.Services.Relay;
using Unity.Networking.Transport.Relay;
using Unity.Networking.Transport;
using Unity.Services.Relay.Models;
using Unity.Collections;
using System.Collections;
using System;
using PokerParty_SharedDLL;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager instance;

    public NetworkDriver NetworkDriver;
    private NetworkConnection connection;

    private string joinCode;

    private async void Awake()
    {
        if (instance == null)
        { 
            instance = this;
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

    private void UpdatePlayer()
    {
        if (!NetworkDriver.IsCreated || !NetworkDriver.Bound)
        {
            return;
        }

        NetworkDriver.ScheduleUpdate().Complete();

        NetworkEvent.Type eventType;
        while ((eventType = connection.PopEvent(NetworkDriver, out var stream)) != NetworkEvent.Type.Empty)
        {
            switch (eventType)
            {
                case NetworkEvent.Type.Data:
                    NetworkMessageType type = (NetworkMessageType)Enum.ToObject(typeof(NetworkMessageType), stream.ReadUInt());

                    FixedString512Bytes msg = stream.ReadFixedString512();
                    string data = msg.ToString();

                    Debug.Log($"Data received: {data}");

                    NetworkMessageHandler.ProcessMessage(type, data);
                    break;

                case NetworkEvent.Type.Connect:
                    Debug.Log("Player connected to the Host");
                    MessageSender.SendMessageToHost(new ConnectionMessage());
                    NetworkingGUI.instance.ShowJoinedPanel(true);
                    break;

                case NetworkEvent.Type.Disconnect:
                    connection = default(NetworkConnection);
                    if (NetworkingGUI.instance != null)
                    {
                        NetworkingGUI.instance.ShowJoinedPanel(false);
                        NetworkingGUI.instance.ResetReadyButton();
                        PopupManager.instance.ShowPopup(PopupType.ErrorPopup, "You got disconnected from the game");
                    }

                    if (GameOverGUI.instance != null)
                    {
                        GameOverGUI.instance.ShowGameOverPanel();
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

            if (NetworkDriver.IsCreated)
            {
                NetworkDriver.Dispose();
            }

            NetworkDriver = NetworkDriver.Create(settings);

            BindToHost(NetworkDriver);
        }
        catch (Exception e)
        {
            NetworkingGUI.instance.ShowJoinError(e.Message);
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
        MessageSender.Initialize(networkDriver, connection);

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
        if (NetworkDriver.IsCreated)
        {
            NetworkDriver.Dispose();
        }
    }
}
