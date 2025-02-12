using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameGUI : MonoBehaviour
{
    public static GameGUI instance;

    public TextMeshProUGUI moneyText;
    public GameObject inGamePanel;
    public Button disconnectBtn;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    
    private void Start()
    {
        disconnectBtn.onClick.AddListener(() => StartCoroutine(OnDisconnectBtnClick()));
    }

    public void StartGame()
    {
        Loader.instance.StopLoading();
        inGamePanel.SetActive(true);
    }

    public void UpdateMoney()
    {
        moneyText.text = $"{GameManager.instance.money}$";
    }

    private IEnumerator OnDisconnectBtnClick()
    {
        yield return GameManager.instance.DisconnectFromGame();
    }
}
