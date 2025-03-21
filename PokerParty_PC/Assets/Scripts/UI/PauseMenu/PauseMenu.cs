using System.Collections.Generic;
using PokerParty_SharedDLL;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Serialization;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button endGameBtn;
    [SerializeField] private GameObject pausePanel;
    
    [HideInInspector] public bool isPaused;

    public TurnDoneMessage turnDoneMessageReceivedDuringPause;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        
        resumeBtn.onClick.AddListener(Resume);
        endGameBtn.onClick.AddListener(() => StartCoroutine(EndGame()));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                AudioManager.instance.PlayMenuClick();
                Pause();
            }
        }
    }
    
    private void Pause()
    {
        Loader.instance.StopLoading();
        Time.timeScale = 0;
        isPaused = true;
        pausePanel.SetActive(true);
        ConnectionManager.instance.SendMessageToAllConnections(new GamePausedMessage());
    }
    
    private void Resume()
    {
        Time.timeScale = 1;
        isPaused = false;
        pausePanel.SetActive(false);
        TurnManager.instance.HandleTurnDone(turnDoneMessageReceivedDuringPause);
        ConnectionManager.instance.SendMessageToAllConnections(new GameUnpausedMessage());
    }
    
    private IEnumerator EndGame()
    {
        Time.timeScale = 1;
        yield return GameManager.instance.DisconnectPlayersAndLoadMainMenu();
    }

}