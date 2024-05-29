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

public class RelayManager : MonoBehaviour
{
    [SerializeField] private Button createJoinCodeButton;
    [SerializeField] private TextMeshProUGUI joinCodeDisplayText;

    public NetworkDriver networkDriver;
    public NativeList<NetworkConnection> connections;

    private async void Awake()
    {
        await InitializeUnityServices();
        joinCodeDisplayText.text = "";
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

        // Clean up stale connections.
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
            {
                Debug.Log("Stale connection removed");
                connections.RemoveAt(i);
                --i;
            }
        }

        NetworkConnection incomingConnection;
        while ((incomingConnection = networkDriver.Accept()) != default(NetworkConnection))
        {
            Debug.Log("Accepted an incoming connection.");
            connections.Add(incomingConnection);
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

            connections = new NativeList<NetworkConnection>(maxConnections, Allocator.Persistent);

            RelayServerData relayServerData = new RelayServerData(allocation, "udp");

            var settings = new NetworkSettings();
            settings.WithRelayParameters(ref relayServerData);

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

            joinCodeDisplayText.text = joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"Error creating join code: {e.Message}");
            ShowJoinCodeError("Failed to create join code. Please try again.");
        }
    }


    private void ShowJoinCodeError(string errorMessage)
    {
        Debug.LogError(errorMessage);
    }
}
