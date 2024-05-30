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

    [SerializeField] private AuthManager authManager;

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

    private async void RegisterAccount()
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

        await authManager.Register(username, password);
        ShowLoginPanel();
    }
}
