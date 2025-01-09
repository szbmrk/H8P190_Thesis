using PokerParty_SharedDLL;
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
    public NativeList<NetworkConnection> Connections;

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

    void UpdateHost()
    {
        if (!NetworkDriver.IsCreated || !NetworkDriver.Bound )
        {
            return;
        }

        NetworkDriver.ScheduleUpdate().Complete();

        NetworkConnection incomingConnection;
        while ((incomingConnection = NetworkDriver.Accept()) != default(NetworkConnection))
        {
            Debug.Log("Accepted an incoming connection.");
            Connections.Add(incomingConnection);
        }

        // Process events from all connections.
        for (int i = 0; i < Connections.Length; i++)
        {
            Assert.IsTrue(Connections[i].IsCreated);

            NetworkEvent.Type eventType;
            while ((eventType = NetworkDriver.PopEventForConnection(Connections[i], out var stream)) != NetworkEvent.Type.Empty)
            {
                switch (eventType)
                {
                    case NetworkEvent.Type.Data:
                        NetworkMessageType type = (NetworkMessageType)Enum.ToObject(typeof(NetworkMessageType), stream.ReadUInt());

                        FixedString512Bytes msg = stream.ReadFixedString512();
                        string data = msg.ToString();

                        Debug.Log($"Type: {type}");
                        Debug.Log($"Data received: {data}");

                        NetworkMessageHandler.ProcessMessage(type, data, i);
                        break;
                }
            }
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
        }
        catch (Exception e)
        {
            Loader.instance.StopLoading();
            PopupManager.instance.ShowPopup(PopupType.ErrorPopup, e.Message);
        }
    }

    private void BindTheHost(Allocation allocation, int maxConnections)
    {
        RelayServerData relayServerData = new RelayServerData(allocation, "udp");

        var settings = new NetworkSettings();
        settings.WithRelayParameters(ref relayServerData);

        if (NetworkDriver.IsCreated)
            NetworkDriver.Dispose();

        NetworkDriver = NetworkDriver.Create(settings);
        if (NetworkDriver.Bind(NetworkEndpoint.AnyIpv4) != 0)
        {
            throw new Exception("Host client failed to bind");
        }
        else
        {
            if (NetworkDriver.Listen() != 0)
            {
                throw new Exception("Host client failed to listen");
            }
            else
            {
                Debug.Log("Host client bound to Relay server");
            }
        }

        if (Connections.IsCreated)
            Connections.Dispose();

        Connections = new NativeList<NetworkConnection>(maxConnections, Allocator.Persistent);
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
        if (Connections[index].IsCreated)
        {
            Debug.Log("Player disconnected from host");
            NetworkDriver.Disconnect(Connections[index]);
            Connections[index] = default(NetworkConnection);
            Connections.RemoveAt(index);
        }
    }

    public void SendMessageToAllConnections(ANetworkMessagePC message)
    {
        for (int i = 0; i < Connections.Length; i++)
        {
            SendMessageToConnection(Connections[i], message);
        }
    }

    public void SendMessageToConnection(NetworkConnection connection, ANetworkMessagePC message)
    {
        string messageInString = JsonUtility.ToJson(message);

        if (NetworkDriver.BeginSend(connection, out DataStreamWriter writer) == 0)
        {
            writer.WriteUInt((uint)message.Type);
            writer.WriteFixedString512(messageInString);
            NetworkDriver.EndSend(writer);
            Debug.Log($"Message sent: {messageInString}");
        }
    }

    private IEnumerator DisposeDriver()
    {
        yield return new WaitForSeconds(1f);
        if (NetworkDriver.IsCreated)
        {
            Debug.Log("Host disposed");
            NetworkDriver.Dispose();
        }
    }

    private IEnumerator DisposeConnections()
    {
        yield return new WaitForSeconds(1f);
        if (Connections.IsCreated)
        {
            Debug.Log("Connections disposed");
            Connections.Dispose();
        }
    }

    public IEnumerator DisposeDriverAndConnections()
    {
        yield return DisposeDriver();
        yield return DisposeConnections();
    }
}