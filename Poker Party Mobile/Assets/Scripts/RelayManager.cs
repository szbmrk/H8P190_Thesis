using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using System.Threading.Tasks;

public class RelayManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TextMeshProUGUI signedInPlayerDisplay;
    [SerializeField] private Button signInBtn;
    [SerializeField] private Button joinBtn;

    string joinCode;

    private async void Awake()
    {
        await InitializeUnityServices();
        joinBtn.onClick.AddListener(JoinGame);
        signedInPlayerDisplay.text = "";
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

    private async void JoinGame()
    {
        try
        {
            joinCode = joinCodeInputField.text.Trim();
            await RelayService.Instance.JoinAllocationAsync(joinCode);
            StartGame();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"Error joining game: {e.Message}");
            ShowJoinCodeError("Failed to join game with code: " + joinCode);
        }
    }

    private void StartGame()
    {
        Debug.Log("Game started!");
    }

    private void ShowJoinCodeError(string errorMessage)
    {
        Debug.LogError(errorMessage);
    }
}
