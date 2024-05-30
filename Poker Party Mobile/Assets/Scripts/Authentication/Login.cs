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

    [SerializeField] private AuthManager authManager;
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

        Debug.Log($"Username: {username}, Password: {password}");

        await authManager.Login(username, password);
        SceneManager.LoadScene("Game");
    }
}
