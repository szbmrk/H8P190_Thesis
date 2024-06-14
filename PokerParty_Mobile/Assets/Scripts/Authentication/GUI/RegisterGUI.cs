using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterGUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_InputField passwordAgainInputField;

    [SerializeField] private Button alreadyHaveAnAccountBtn;
    [SerializeField] private Button signInBtn;

    [SerializeField] private GameObject loginPanel;

    private void Awake()
    {
        Loader.Instance.StopLoading();
        alreadyHaveAnAccountBtn.onClick.AddListener(ShowLoginPanel);
        signInBtn.onClick.AddListener(RegisterButtonClick);
    }

    private void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    private void RegisterButtonClick()
    {
        signInBtn.interactable = false;
        string username = playerNameInputField.text;
        string password = passwordInputField.text;
        string passwordAgain = passwordAgainInputField.text;

        RegisterAccount(username, password, passwordAgain);
        signInBtn.interactable = true;
    }

    private async void RegisterAccount(string playerName, string password, string passwordAgain)
    {
        Loader.Instance.StartLoading();

        if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Username or password is empty");
            return;
        }

        if (password != passwordAgain)
        {
            Debug.LogError("Passwords do not match");
            return;
        }

        try
        {
            await AuthManager.Instance.Register(playerName, password);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            playerNameInputField.text = "";
            passwordInputField.text = "";
            passwordAgainInputField.text = "";
            Loader.Instance.StopLoading();
            return;
        }

        ShowLoginPanel();
    }
}
