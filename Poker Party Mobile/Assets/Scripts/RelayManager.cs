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

public class RelayManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TextMeshProUGUI signedInPlayerDisplay;
    [SerializeField] private Button joinBtn;

    public NetworkDriver networkDriver;
    public NetworkConnection connection;
    string joinCode;

    private async void Awake()
    {
        await InitializeUnityServices();
        networkDriver = NetworkDriver.Create();
        joinBtn.onClick.AddListener(JoinRelay);
        signedInPlayerDisplay.text = AccountManager.LoggedInAccount.Username;
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
                    break;

                case NetworkEvent.Type.Disconnect:
                    Debug.Log("Player got disconnected from the Host");
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

    private async void JoinRelay()
    {
        try
        {
            joinCode = joinCodeInputField.text.Trim();
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "udp");
            var settings = new NetworkSettings();
            settings.WithRelayParameters(ref relayServerData);

            networkDriver = NetworkDriver.Create(settings);

            if (networkDriver.Bind(NetworkEndPoint.AnyIpv4) != 0)
            {
                Debug.LogError("Player client failed to bind");
            }
            else
            {
                Debug.Log("Player client bound to Relay server");
            }

            connection = networkDriver.Connect();
            Debug.Log("Joined Relay with Join Code: " + joinCode);
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"Error joining game: {e.Message}");
            ShowJoinCodeError("Failed to join game with code: " + joinCode);
        }
    }

    private void ShowJoinCodeError(string errorMessage)
    {
        Debug.LogError(errorMessage);
    }

    private void OnDestroy()
    {
        if (connection != null)
        {
            networkDriver.Dispose();
            connection.Disconnect(networkDriver);
            connection = default;
        }
    }
}
