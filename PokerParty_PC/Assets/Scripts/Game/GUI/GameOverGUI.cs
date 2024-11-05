using System;
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
        backToMainBtn.onClick.AddListener(OnBackToMainBtnClick);
    }

    public void Open(string WinnerName)
    {
        winnerNameText.text = WinnerName;
        gameOverPanel.SetActive(true);
    }

    private void OnBackToMainBtnClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
