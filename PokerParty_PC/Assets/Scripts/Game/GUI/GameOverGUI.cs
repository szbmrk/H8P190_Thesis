using PokerParty_SharedDLL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverGUI : MonoBehaviour
{
    public static GameOverGUI Instance;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button backToMainBtn;
    [SerializeField] private TextMeshProUGUI winnerNameText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
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
        ConnectionManager.Instance.DisconnectAllPlayers();
        yield return ConnectionManager.Instance.DisposeDriverAndConnections();
        Destroy(ConnectionManager.Instance.gameObject);
        SceneManager.LoadScene("MainMenu");
    }
}
