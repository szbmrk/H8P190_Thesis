using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginGUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TMP_InputField passwordInputField;

    [SerializeField] private Button dontHaveAnAccountBtn;
    [SerializeField] private Button loginBtn;
    [SerializeField] private Button forgotPasswordBtn;
    [SerializeField] private Button forgotPlayerName;

    [SerializeField] private Toggle KeepMeLoggedInCheckBox;

    [SerializeField] private GameObject registerPanel;
    private void Awake()
    {
        Loader.Instance.StopLoading();

        dontHaveAnAccountBtn.onClick.AddListener(ShowRegisterPanel);
        loginBtn.onClick.AddListener(LoginButtonClick);
        forgotPasswordBtn.onClick.AddListener(ForgotPasswordButtonClick);
        forgotPlayerName.onClick.AddListener(ForgotPlayerNameButtonClick);

        if (PlayerPrefs.HasKey("playerName") && PlayerPrefs.HasKey("password"))
        {
            Debug.Log("Logging in automatically");
            LoginToAccount(PlayerPrefs.GetString("playerName"), PlayerPrefs.GetString("password"));
        }

    }

    private void ShowRegisterPanel()
    {
        registerPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    private void LoginButtonClick()
    {
        loginBtn.interactable = false;
        string username = playerNameInputField.text;
        string password = passwordInputField.text;

        LoginToAccount(username, password);
        loginBtn.interactable = true;
    }

    private async void LoginToAccount(string playerName, string password)
    {
        Loader.Instance.StartLoading();

        if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(password))
        {
            PopupManager.Instance.ShowPopup(PopupType.ErrorPopup, "PlayerName or password is empty");
            Loader.Instance.StopLoading();
            return;
        }

        try
        {
            if (KeepMeLoggedInCheckBox.isOn)
            {
                Debug.Log("Saving login data");
                PlayerPrefs.SetString("playerName", playerName);
                PlayerPrefs.SetString("password", password);
            }

            await AuthManager.Login(playerName, password);
        }
        catch (Exception e)
        {
            playerNameInputField.text = "";
            passwordInputField.text = "";
            PlayerPrefs.DeleteKey("playerName");
            PlayerPrefs.DeleteKey("password");
            Loader.Instance.StopLoading();
            PopupManager.Instance.ShowPopup(PopupType.ErrorPopup, e.Message);
            return;
        }

        SceneManager.LoadScene("Lobby");
    }

    private void ForgotPasswordButtonClick()
    {
        PopupManager.Instance.ShowInputPopup("Password Reset", "Enter email or playerName", "Send email", SendForgotPasswordEmail);
    }

    private void ForgotPlayerNameButtonClick()
    {
        PopupManager.Instance.ShowInputPopup("Player Name Reminder", "Enter email", "Send email", SendForgotPlayerNameEmail);
    }

    private async Task<bool> SendForgotPasswordEmail()
    {
        Loader.Instance.StartLoading();

        try
        {
            string emailOrPlayername  = PopupManager.Instance.currentInputPopup.inputField.text;
            string response = await PasswordResetManager.SendPasswordResetEmail(emailOrPlayername);
            PopupManager.Instance.ShowPopup(PopupType.SuccessPopup, response);
        }
        catch (Exception e)
        {
            Loader.Instance.StopLoading();
            PopupManager.Instance.ShowPopup(PopupType.ErrorPopup, e.Message);
            return false;
        }

        Loader.Instance.StopLoading();
        return true;
    }

    private async Task<bool> SendForgotPlayerNameEmail()
    {
        Loader.Instance.StartLoading();

        try
        {
            string email = PopupManager.Instance.currentInputPopup.inputField.text;
            string response = await ForgotPlayerNameManager.SendPlayerNameReminderEmail(email);
            PopupManager.Instance.ShowPopup(PopupType.SuccessPopup, response);
        }
        catch (Exception e)
        {
            Loader.Instance.StopLoading();
            PopupManager.Instance.ShowPopup(PopupType.ErrorPopup, e.Message);
            return false;
        }

        Loader.Instance.StopLoading();
        return true;
    }
}
