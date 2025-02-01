using TMPro;
using UnityEngine;

public class GameGUI : MonoBehaviour
{
    public static GameGUI instance;

    public TextMeshProUGUI moneyText;
    public GameObject inGamePanel;

    private void Awake()
    {
        if (instance == null)
            instance = this;
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
}
