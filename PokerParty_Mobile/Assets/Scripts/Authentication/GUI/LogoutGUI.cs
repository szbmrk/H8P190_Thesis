using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogoutGUI : MonoBehaviour
{
    public static LogoutGUI Instance;

    [SerializeField] private Button logoutBtn;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        logoutBtn.onClick.AddListener(Logout);
    }

    private void Logout()
    {
        PlayerPrefs.DeleteKey("playerName");
        PlayerPrefs.DeleteKey("password");

        if (ConnectionManager.Instance != null)
            Destroy(ConnectionManager.Instance.gameObject);

        SceneManager.LoadScene("Authentication");
    }

    public void ShowLogoutBtn()
    {
        logoutBtn.gameObject.SetActive(true);
    }

    public void HideLogoutBtn()
    {
        logoutBtn.gameObject.SetActive(false);
    }
}
