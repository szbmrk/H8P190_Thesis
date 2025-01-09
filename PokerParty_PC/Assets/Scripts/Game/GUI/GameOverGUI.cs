using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverGUI : MonoBehaviour
{
    public static GameOverGUI instance;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button backToMainBtn;
    [SerializeField] private TextMeshProUGUI winnerNameText;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        backToMainBtn.onClick.AddListener(() => StartCoroutine(OnBackToMainBtnClick()));
    }

    public void Open(string winnerName)
    {
        winnerNameText.text = winnerName;
        gameOverPanel.SetActive(true);
    }

    private IEnumerator OnBackToMainBtnClick()
    {
        Loader.Instance.StartLoading();
        ConnectionManager.instance.DisconnectAllPlayers();
        yield return ConnectionManager.instance.DisposeDriverAndConnections();
        Destroy(ConnectionManager.instance.gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}
