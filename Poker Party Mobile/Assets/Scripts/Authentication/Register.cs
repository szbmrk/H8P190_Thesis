using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Register : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_InputField passwordAgainInputField;

    [SerializeField] private Button alreadyHaveAnAccountBtn;
    [SerializeField] private Button signInBtn;

    [SerializeField] private GameObject loginPanel;

    private void Awake()
    {
        alreadyHaveAnAccountBtn.onClick.AddListener(ShowLoginPanel);
        signInBtn.onClick.AddListener(RegisterAccount);
    }

    private void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    private void RegisterAccount()
    {
        string username = usernameInputField.text;
        string password = passwordInputField.text;
        string passwordAgain = passwordAgainInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Username or password is empty");
            return;
        }

        if (password != passwordAgain)
        {
            Debug.LogError("Passwords do not match");
            return;
        }

        Debug.Log($"Username: {username}, Password: {password}");

        // Register the user
    }
}
