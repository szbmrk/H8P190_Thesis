using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button createGameBtn;
    [SerializeField] private Button quitGameBtn;
    [SerializeField] private Button settingsBtn;

    private void Awake()
    {
        createGameBtn.onClick.AddListener(CreateGame);
        quitGameBtn.onClick.AddListener(QuitGame);
        settingsBtn.onClick.AddListener(OpenSettings);
    }

    private void CreateGame()
    {
        SceneManager.LoadScene("Lobby");
    }
    
    private void QuitGame()
    {
        Application.Quit();
    }

    private void OpenSettings()
    {
        //TODO: Implement settings menu
    }
}
