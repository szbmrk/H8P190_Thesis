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
    [SerializeField] private TextMeshProUGUI placeText;

    private void Awake()
    {
        instance = this;
        backToMainBtn.onClick.AddListener(() => StartCoroutine(OnBackToMainBtnClick()));
    }

    public void ShowGameOverPanel(int place)
    {
        placeText.text = place switch
        {
            1 => "1st",
            2 => "2nd",
            3 => "3rd",
            _ => place + "th"
        };

        if (gameOverPanel.activeInHierarchy)
            return;

        GameGUI.instance.inGamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    private IEnumerator OnBackToMainBtnClick()
    {
        yield return GameManager.instance.GoBackToMainMenu();
    }
}
