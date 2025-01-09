using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverGUI : MonoBehaviour
{
    public static GameOverGUI instance;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button backToMainBtn;

    private void Awake()
    {
        instance = this;
        backToMainBtn.onClick.AddListener(() => StartCoroutine(OnBackToMainBtnClick()));
    }

    public void ShowGameOverPanel()
    {
        if (gameOverPanel.activeInHierarchy)
            return;

        GameGUI.instance.inGamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    private IEnumerator OnBackToMainBtnClick()
    {
        Loader.instance.StartLoading();
        yield return ConnectionManager.instance.DisposeNetworkDriver();
        Destroy(ConnectionManager.instance.gameObject);
        SceneManager.LoadScene("Lobby");
    }
}
