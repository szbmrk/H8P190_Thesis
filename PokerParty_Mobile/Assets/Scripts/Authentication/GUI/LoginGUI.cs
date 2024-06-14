using System;
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

    [SerializeField] private Toggle KeepMeLoggedInCheckBox;

    [SerializeField] private GameObject registerPanel;
    private void Awake()
    {
        dontHaveAnAccountBtn.onClick.AddListener(ShowRegisterPanel);
        loginBtn.onClick.AddListener(LoginButtonClick);

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
        string username = playerNameInputField.text;
        string password = passwordInputField.text;

        LoginToAccount(username, password);
    }

    private async void LoginToAccount(string playerName, string password)
    {
        if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("PlayerName or password is empty");
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

            await AuthManager.Instance.Login(playerName, password);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            playerNameInputField.text = "";
            passwordInputField.text = "";
            PlayerPrefs.DeleteKey("playerName");
            PlayerPrefs.DeleteKey("password");
            return;
        }

        SceneManager.LoadScene("Game");
    }
}
