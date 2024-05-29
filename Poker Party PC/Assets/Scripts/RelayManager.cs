using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.UI;

public class RelayManager : MonoBehaviour
{
    [SerializeField] private Button createJoinCodeButton;
    [SerializeField] private TextMeshProUGUI joinCodeDisplayText;

    private async void Awake()
    {
        await InitializeUnityServices();
        joinCodeDisplayText.text = "";
        createJoinCodeButton.onClick.AddListener(CreateJoinCode);
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

    private async void CreateJoinCode()
    {
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(8);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            joinCodeDisplayText.text = $"Join Code: {joinCode}";
            Debug.Log($"Join Code: {joinCode}");
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
