using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        backToMainBtn.onClick.AddListener(OnBackToMainBtnClick);
    }

    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }

    private void OnBackToMainBtnClick()
    {
        SceneManager.LoadScene("Lobby");
    }
}
