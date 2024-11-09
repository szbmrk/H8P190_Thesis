using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverGUI : MonoBehaviour
{
    public static GameOverGUI Instance;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button backToMainBtn;

    private void Awake()
    {
        Instance = this;
        backToMainBtn.onClick.AddListener(() => StartCoroutine(OnBackToMainBtnClick()));
    }

    public void ShowGameOverPanel()
    {
        if (gameOverPanel.activeInHierarchy)
            return;

        GameGUI.Instance.inGamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    private IEnumerator OnBackToMainBtnClick()
    {
        Loader.Instance.StartLoading();
        yield return ConnectionManager.Instance.DisposeNetworkDriver();
        Destroy(ConnectionManager.Instance.gameObject);
        SceneManager.LoadScene("Lobby");
    }
}
