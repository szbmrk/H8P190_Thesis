using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button createGameBtn;
    [SerializeField] private Button quitGameBtn;

    private void Awake()
    {
        Time.timeScale = 1;
        createGameBtn.onClick.AddListener(CreateGame);
        quitGameBtn.onClick.AddListener(QuitGame);
    }

    private void CreateGame()
    {
        SceneManager.LoadScene("Lobby");
    }
    
    private void QuitGame()
    {
        Application.Quit();
    }
}
