using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button createGameBtn;

    private void Awake()
    {
        createGameBtn.onClick.AddListener(CreateGame);
    }

    private void CreateGame()
    {
        SceneManager.LoadScene("Lobby");
    }
}
