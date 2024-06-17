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
using UnityEngine.UI;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Instance;

    public NetworkDriver networkDriver;
    public NativeList<NetworkConnection> Connections;

    private async void Start()
    {
        if (Instance == null)
            Instance = this;

        await AuthAndServicesManager.InitializeUnityServices();
        CreateRelayAndJoinCode();
    }

    private void Update()
    {
        UpdateHost();
    }

    void UpdateHost()
    {
        if (!networkDriver.IsCreated || !networkDriver.Bound )
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

                        NetworkMessageManager.ProcessMesage(type, data, i);
                        break;
                }
            }
        }
    }

    private async void CreateRelayAndJoinCode(int maxConnections = 8)
    {
        try
        {
            Loader.Instance.StartLoading();

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            BindTheHost(allocation, maxConnections);

            LobbyGUI.Instance.joinCodeText.text = joinCode;
            LobbyGUI.Instance.ShowPanel();
            Loader.Instance.StopLoading();
        }
        catch (Exception e)
        {
            Loader.Instance.StopLoading();
            PopupManager.Instance.ShowPopup(PopupType.ErrorPopup, e.Message);
        }
    }

    private void BindTheHost(Allocation allocation, int maxConnections)
    {
        RelayServerData relayServerData = new RelayServerData(allocation, "udp");

        var settings = new NetworkSettings();
        settings.WithRelayParameters(ref relayServerData);

        if (networkDriver.IsCreated)
            networkDriver.Dispose();

        networkDriver = NetworkDriver.Create(settings);
        if (networkDriver.Bind(NetworkEndPoint.AnyIpv4) != 0)
        {
            throw new Exception("Host client failed to bind");
        }
        else
        {
            if (networkDriver.Listen() != 0)
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
            networkDriver.Disconnect(Connections[index]);
            Connections[index] = default(NetworkConnection);
            Connections.RemoveAt(index);
        }
    }

    private IEnumerator DisposeDriver()
    {
        yield return new WaitForSeconds(1f);
        if (networkDriver.IsCreated)
        {
            Debug.Log("Host disposed");
            networkDriver.Dispose();
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