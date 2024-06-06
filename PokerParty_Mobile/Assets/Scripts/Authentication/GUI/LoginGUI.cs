using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginGUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;

    [SerializeField] private Button dontHaveAnAccountBtn;
    [SerializeField] private Button loginBtn;

    [SerializeField] private GameObject registerPanel;
    private void Awake()
    {
        dontHaveAnAccountBtn.onClick.AddListener(ShowRegisterPanel);
        loginBtn.onClick.AddListener(loginToAccount);
    }

    private void ShowRegisterPanel()
    {
        registerPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    private async void loginToAccount()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Username or password is empty");
            return;
        }

        try
        {
            await AuthManager.Instance.Login(username, password);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            usernameInputField.text = "";
            passwordInputField.text = "";
            return;
        }

        SceneManager.LoadScene("Game");
    }
}
