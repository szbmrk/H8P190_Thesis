using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
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

    private void loginToAccount()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Username or password is empty");
            return;
        }

        Debug.Log($"Username: {username}, Password: {password}");

        AccountManager.LoggedInAccount = new Account(username);
        SceneManager.LoadScene("Game");
    }
}
