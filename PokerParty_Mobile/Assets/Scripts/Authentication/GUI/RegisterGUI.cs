using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterGUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInputField;
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
        string email = emailInputField.text;
        string username = playerNameInputField.text;
        string password = passwordInputField.text;
        string passwordAgain = passwordAgainInputField.text;

        RegisterAccount(email, username, password, passwordAgain);
        signInBtn.interactable = true;
    }

    private async void RegisterAccount(string email, string playerName, string password, string passwordAgain)
    {
        Loader.Instance.StartLoading();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(password))
        {
            PopupManager.Instance.ShowPopup(PopupType.ErrorPopup, "Email or PlayerName or password is empty");
            return;
        }

        if (password != passwordAgain)
        {
            PopupManager.Instance.ShowPopup(PopupType.ErrorPopup, "Passwords do not match");
            return;
        }

        try
        {
            await AuthManager.Instance.Register(email, playerName, password);
        }
        catch (Exception e)
        {
            ResetFields();
            Loader.Instance.StopLoading();
            PopupManager.Instance.ShowPopup(PopupType.ErrorPopup, e.Message);
            return;
        }

        Loader.Instance.StopLoading();
        ShowLoginPanel();
    }

    private void ResetFields()
    {
        emailInputField.text = "";
        playerNameInputField.text = "";
        passwordInputField.text = "";
        passwordAgainInputField.text = "";
    }
}
